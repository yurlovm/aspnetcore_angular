using System.Linq;
using WEBAPP.Models;

namespace WEBAPP.DAL
{
    public class CrisesRepository : ICrisesRepository
    {
        private Crisis[] crises = new Crisis[]
        {
              new Crisis { Id = 1, Name = "Dragon Burning Cities" },
              new Crisis { Id = 2, Name = "Sky Rains Great White Sharks" },
              new Crisis { Id = 3, Name = "Giant Asteroid Heading For Earth" },
              new Crisis { Id = 4, Name = "Procrastinators Meeting Delayed Again" }
        };

        public Crisis[] GetAllCrises()
        {
            return crises;
        }

        public Crisis GetCrisisById(int id)
        {
            return crises.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}