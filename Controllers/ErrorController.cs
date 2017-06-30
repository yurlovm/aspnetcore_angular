using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WEBAPP.Controllers
{
    [Route("/Error")]
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            // Handle error here
            return NotFound();
        }
    }
}