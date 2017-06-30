using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace WEBAPP.Infrastructure
{
    public class CustomRoleManager : RoleManager<CustomRole>
    {
        /// <summary>
        /// Managing roles in Identity system
        /// </summary>
        public CustomRoleManager(IRoleStore<CustomRole> store, IEnumerable<IRoleValidator<CustomRole>> roleValidators, ILookupNormalizer keyNormalizer, 
                              IdentityErrorDescriber errors, ILogger<RoleManager<CustomRole>> logger, IHttpContextAccessor contextAccessor) 
            : base(store, roleValidators, keyNormalizer, errors, logger, contextAccessor)
        {
            
        }
    }
}
