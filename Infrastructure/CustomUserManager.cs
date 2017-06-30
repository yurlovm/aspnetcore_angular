using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace WEBAPP.Infrastructure
{
    /// <summary>
    /// Managing users in Identity system 
    /// </summary>
    public class CustomUserManager : UserManager<CustomUser>
    {
        private readonly IPasswordHasher<CustomUser> _passwordHasher;
        private readonly IUserStore<CustomUser> _store;

        public override bool SupportsUserLockout => true;

        public CustomUserManager(IUserStore<CustomUser> store, IOptions<IdentityOptions> optionsAccessor,
                              IPasswordHasher<CustomUser> passwordHasher, IEnumerable<IUserValidator<CustomUser>> userValidators,
                              IEnumerable<IPasswordValidator<CustomUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
                              IServiceProvider services, ILogger<UserManager<CustomUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _passwordHasher = passwordHasher;
            _store = store;
        }

        public override Task<bool> CheckPasswordAsync(CustomUser user, string password)
        {
            return Task.Run(() =>
            {
                var password_hash = _passwordHasher.HashPassword(user, password);
                PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, password_hash, password);
                return result == PasswordVerificationResult.Success ? true : false;
            });
        }

    }
}
