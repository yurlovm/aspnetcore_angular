
using System;

namespace WEBAPP.Infrastructure
{
    public class CustomUser
    {
        public CustomUser(string name)
        {
            UserName = name;
            AccessFailedCount = 0;
            IsBlocked = false;
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsBlocked { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
    }
}



