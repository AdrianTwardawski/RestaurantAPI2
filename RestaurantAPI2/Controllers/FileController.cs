using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2.Controllers
{
    [Route("file")]
    [Authorize]
    public class FileController : ControllerBase
    {
        public ActionResult GetFile([FromQuery]string fileName)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = $"{rootPath}/PrivateFiles/{fileName}";
            var fileExists = System.IO.File.Exists(filePath);
            if(!fileExists)
            {
                return NotFound();
            }
            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(fileName, out string contentType);
            var fileContents = System.IO.File.ReadAllBytes(filePath);
            return File(fileContents, contentType, fileName);
        }
    }
}
