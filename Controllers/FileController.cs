using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace WEBAPP.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly ILogger _logger;
        private readonly IFileProvider _fileProvider;
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileController(ILogger<ValuesController> logger, IFileProvider fileProvider, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _fileProvider = fileProvider;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("codes")]
        public IActionResult GetRegionCodes()
        {
            string fileName = "codes.json";
            IFileInfo fileInfo = _fileProvider.GetFileInfo($"Assets/{fileName}");
            Stream readStream = fileInfo.CreateReadStream();
            return File(readStream, "application/json", fileName);
        }

        [HttpGet("json/{code}")]
        public IActionResult GetTopojson(string code)
        {
            string fileName = $"{code}.json";
            IFileInfo fileInfo = _fileProvider.GetFileInfo($"Assets/json/{fileName}");
            Stream readStream = fileInfo.CreateReadStream();
            return File(readStream, "application/json", fileName);
        }
    }
}

