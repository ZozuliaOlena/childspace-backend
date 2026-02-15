using Microsoft.AspNetCore.Mvc;
using childspace_backend.Repositories;
using childspace_backend.Models.DTOs;
using System.Net;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryRepository _cloudinaryRepository;

        public CloudinaryController(ICloudinaryRepository cloudinaryRepository)
        {
            _cloudinaryRepository = cloudinaryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var result = await _cloudinaryRepository.UploadAsync(file);

            if (result == null)
            {
                return Problem("Upload failed", null, (int)HttpStatusCode.InternalServerError);
            }

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return BadRequest("PublicId is required");
            }

            var isDeleted = await _cloudinaryRepository.DeleteAsync(publicId);

            if (isDeleted)
            {
                return Ok(new { message = "File deleted successfully" });
            }

            return BadRequest("Could not delete file (check PublicId)");
        }
    }
}