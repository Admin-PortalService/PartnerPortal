using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestWebApplication.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly string UploadsDirectory = "wwwroot/uploads"; // Change this to your desired upload directory.

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    //var uniqueFileName = Guid.NewGuid() + "_" + file.FileName; // Generate a unique file name to avoid conflicts.
                    var fileName = file.FileName;
                    var uploadPath = Path.Combine(UploadsDirectory, fileName);

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Return a JSON response with the file URL.
                    var fileUrl = Url.Content("~/uploads/" + fileName);
                    return Ok(new { url = fileUrl });
                }
                return BadRequest("Invalid file or no file provided.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
