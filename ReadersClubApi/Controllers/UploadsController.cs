using Microsoft.AspNetCore.Mvc;
using ReadersClubApi.Helpers;

namespace ReadersClubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public UploadsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("cover")]
        public IActionResult UploadCover([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var fileName = FileSettings.UploadFile(file, "Covers", _env.WebRootPath);
            var url = $"{Request.Scheme}://{Request.Host}/Uploads/Covers/{fileName}";

            return Ok(new { fileName, url });
        }
    }
}
