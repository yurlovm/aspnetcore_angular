using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEBAPP.DAL;
using WEBAPP.Models;

namespace WEBAPP.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Authorize(Policy = "RequireAdministratorRole")]
    [Route("api/[controller]")]
    public class CrisesController : Controller
    {
        private ICrisesRepository _crisesRepo;

        public CrisesController(ICrisesRepository crisesRepo)
        {
            _crisesRepo = crisesRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            Crisis[] crises = _crisesRepo.GetAllCrises();
            return Ok(crises);
        }

        // GET: api/crises/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            Crisis crisis = _crisesRepo.GetCrisisById(id);
            return Ok(crisis);
        }

        // POST: api/crises
        [HttpPost]
        public IActionResult Post([FromBody]string value)
        {
            return Ok();
        }

        // PUT: api/crises/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]string value)
        {
            return Ok();
        }

        // DELETE: api/crises/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
