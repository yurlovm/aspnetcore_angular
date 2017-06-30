using WEBAPP.Models;

namespace WEBAPP.DAL
{
    public interface IHeroesRepository
    {
        Hero[] GetAllHeroes();
        Hero GetHeroById(int id);
    }
}