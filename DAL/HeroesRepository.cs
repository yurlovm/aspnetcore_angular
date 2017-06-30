using System.Linq;
using WEBAPP.Models;

namespace WEBAPP.DAL
{
    public class HeroesRepository : IHeroesRepository
    {
        private Hero[] heroes = new Hero[]
        {
              new Hero { Id = 11, Name = "Mr. Nice" },
              new Hero { Id = 12, Name = "Narco" },
              new Hero { Id = 13, Name = "Bombasto" },
              new Hero { Id = 14, Name = "Celeritas" },
              new Hero { Id = 15, Name = "Magneta" },
              new Hero { Id = 16, Name = "RubberMan" }
        };

        public Hero[] GetAllHeroes()
        {
            return heroes;
        }

        public Hero GetHeroById(int id)
        {
            return heroes.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}