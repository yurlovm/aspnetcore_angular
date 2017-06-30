using Microsoft.AspNetCore.Mvc;
using WEBAPP.DAL;
using WEBAPP.Models;

namespace WEBAPP.Controllers
{
    [Route("api/[controller]")]
    public class HeroesController : Controller
    {
        private IHeroesRepository _heroesRepo;

        public HeroesController(IHeroesRepository heroesRepo)
        { 
            _heroesRepo = heroesRepo;
        }

        // GET: api/heroes
        [HttpGet]
        public IActionResult Get()
        {
            Hero[] heroes = _heroesRepo.GetAllHeroes();
            return Ok(heroes);
        }

        // GET: api/heroes/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if(id <= 0)
            {
                return NotFound();
            }

            Hero hero = _heroesRepo.GetHeroById(id);
            return Ok(hero);
        }

        // POST: api/heroes
        [HttpPost]
        public IActionResult Post([FromBody]string value)
        {
            return Ok();
        }

        // PUT: api/heroes/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]string value)
        {
            return Ok();
        }

        // DELETE: api/heroes/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
