using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WEBAPP.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace WEBAPP.Controllers
{
    [Route("/oauth")]
    public class OauthController : Controller
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<CustomUser> _userManager;

        public OauthController(IOptions<JwtSettings> jwtSettings, UserManager<CustomUser> userManager)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
        }

        [HttpPost("token")]
        public async Task GetToken()
        {
            var grant_type = Request.Form["grant_type"];
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            if(grant_type != "password")
            {
               Response.StatusCode = 400;
               await Response.WriteAsync("Invalid grant type of request.");
               return;
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                Response.StatusCode = 401;
                await Response.WriteAsync("The username/password couple is invalid.");
                return;
            }

            // Ensure the user is not already locked out.
            if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
            {
                Response.StatusCode = 401;
                await Response.WriteAsync("The username/password couple is invalid.");
                return;
            }

            // Ensure the password is valid.
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                if (_userManager.SupportsUserLockout)
                {
                    await _userManager.AccessFailedAsync(user);
                }

                Response.StatusCode = 401;
                await Response.WriteAsync("The username/password couple is invalid.");
                return;
            }
            if (_userManager.SupportsUserLockout)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            var identity = GetIdentity(user);
            if (identity == null)
            {
                Response.StatusCode = 401;
                await Response.WriteAsync("The username/password couple is invalid.");
                return;
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // Set token expiration time dynamically. All tokens must expire at 01:00 AM Msk time
            var expire_time = DateTime.Today.AddDays(1).AddHours(1);
            int hours_diff = (int)Math.Ceiling((DateTime.Now - DateTime.UtcNow).TotalHours);
            DateTimeOffset? expires = new DateTimeOffset(expire_time, new TimeSpan(hours_diff, 0, 0));

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(subject: identity,
                                                       signingCredentials: signingCredentials,
                                                       audience: _jwtSettings.Audience,
                                                       issuer: _jwtSettings.Issuer,
                                                       expires: expires.Value.UtcDateTime);

            var encodedJwt = handler.WriteToken(token);
            var response = new
            {
                access_token = encodedJwt,
                token_type = "bearer",
            };
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response));
        }


        [Authorize(Roles = "Administrator"), HttpGet("secure")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task Secret()
        {
            var currentUser = HttpContext.User;
            var identity = (ClaimsIdentity)currentUser.Identity;
            await Response.WriteAsync(currentUser.Identity.Name);
            return;
        }

        private ClaimsIdentity GetIdentity(CustomUser user)
        {
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim("user_id", user.Id),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "Administrator"),
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "JWT", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}
