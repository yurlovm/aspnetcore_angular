using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEBAPP.Infrastructure
{
    public class CustomRole
    {
        public CustomRole(string name)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}
