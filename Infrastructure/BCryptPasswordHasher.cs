using System;
using Microsoft.AspNetCore.Identity;

namespace WEBAPP.Infrastructure
{
    /// <summary>
    /// Password hasher serivce by BCrypt 
    /// </summary>
    public class BCryptPasswordHasher : IPasswordHasher<CustomUser>
    {
        private static string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }

        public string HashPassword(CustomUser user, string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GetRandomSalt());
        }

        public PasswordVerificationResult VerifyHashedPassword(CustomUser user, string hashedPassword, string providedPassword)
        {
            if (BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword))
            {
                return PasswordVerificationResult.Success;
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }

        }
    }
}