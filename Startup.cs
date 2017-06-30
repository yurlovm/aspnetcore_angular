using System;
using System.IO;
using System.Text;
using WEBAPP.Infrastructure;
using WEBAPP.WebSockets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WEBAPP.DAL;

namespace WEBAPP
{
    public class Startup
    {
        private IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("keys.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            _hostingEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }

        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPasswordHasher<CustomUser>, BCryptPasswordHasher>(); // must be befor AddIdentity

            services
                .AddIdentity<CustomUser, CustomRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.AllowedForNewUsers = true;
                })
                .AddUserManager<CustomUserManager>()
                .AddRoleManager<CustomRoleManager>()
                .AddUserStore<CustomUserStore>()
                .AddRoleStore<CustomRoleStore>()
                .AddDefaultTokenProviders();

            // Add framework services.
            var jwtSettings = Configuration.GetSection("JWT");
            services.Configure<JwtSettings>(jwtSettings); // Config options as DI service

            services.AddResponseCompression();
            services.AddMvc();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator", "BackupAdministrator"));
                options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber", "1", "2", "3", "4", "5"));
            });
            services.AddWebSocketManager();

            services.AddSingleton<TokenValidationParameters>(new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Get<JwtSettings>().Issuer,
                ValidAudience = jwtSettings.Get<JwtSettings>().Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Get<JwtSettings>().Secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            });

            var physicalProvider = _hostingEnvironment.ContentRootFileProvider;
            services.AddSingleton<IFileProvider>(physicalProvider);
            services.AddSingleton<IHostingEnvironment>(_hostingEnvironment);

            services.AddTransient<ICrisesRepository, CrisesRepository>();
            services.AddTransient<IHeroesRepository, HeroesRepository>();
        }

        // Use this method to configure the HTTP request pipeline. Order of middleware is important
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {

            var validationParameters = serviceProvider.GetService<TokenValidationParameters>();
            var jwtSettings = serviceProvider.GetService<IOptions<JwtSettings>>();

            var options = new JwtBearerOptions
            {
                Audience = jwtSettings.Value.Audience,
                AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme,
                AutomaticAuthenticate = true,
                TokenValidationParameters = validationParameters
            };

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug((category, logLevel) => (category.Contains("WEBAPP") && logLevel >= LogLevel.Trace));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }


            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404 &&
                    !Path.HasExtension(context.Request.Path.Value) &&
                    !context.Request.Path.Value.StartsWith("/api/") &&
                    !context.Request.Path.Value.StartsWith("/oauth/") &&
                    !context.Request.Path.Value.StartsWith("/ws"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });


            app.UseDefaultFiles(); // Serve default index.html
            app.UseStaticFiles();  // Return static files and end pipeline.

            app.UseJwtBearerAuthentication(options);
            app.UseResponseCompression();
            app.UseMvcWithDefaultRoute();

            app.UseWebSockets();
            app.MapWebSocketManager("/ws", serviceProvider.GetService<WebSocketHubHandler>());
        }
    }
}
