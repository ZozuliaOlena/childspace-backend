using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialRepository _repository;
        private readonly ICloudinaryRepository _cloudinaryRepository;

        public MaterialController(IMaterialRepository repository, ICloudinaryRepository cloudinaryRepository)
        {
            _repository = repository;
            _cloudinaryRepository = cloudinaryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAll()
        {
            var materials = await _repository.GetAllAsync();
            return Ok(materials);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MaterialDto>> GetById(Guid id)
        {
            var material = await _repository.GetByIdAsync(id);

            if (material == null)
                return NotFound();

            return Ok(material);
        }

        [HttpPost]
        [RequestSizeLimit(104857600)]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<ActionResult<MaterialDto>> Create([FromForm] MaterialCreateDto dto)
        {
            string finalUrl = "";

            if (dto.File != null)
            {
                var uploadResult = await _cloudinaryRepository.UploadAsync(dto.File);
                if (uploadResult == null) return BadRequest(new { message = "Помилка завантаження файлу" });
                finalUrl = uploadResult.Url;
            }
            else if (!string.IsNullOrEmpty(dto.LinkUrl))
            {
                finalUrl = dto.LinkUrl;
            }
            else
            {
                return BadRequest(new { message = "Будь ласка, додайте файл або вставте посилання" });
            }

            var created = await _repository.CreateAsync(dto, finalUrl);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        [RequestSizeLimit(104857600)]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<ActionResult<MaterialDto>> Update(Guid id, [FromForm] MaterialUpdateDto dto)
        {
            var existingMaterial = await _repository.GetByIdAsync(id);
            if (existingMaterial == null) return NotFound();

            string newFileUrl = existingMaterial.FileUrl;
            bool urlChanged = false;

            if (dto.File != null)
            {
                var uploadResult = await _cloudinaryRepository.UploadAsync(dto.File);
                if (uploadResult == null) return BadRequest(new { message = "Помилка завантаження нового файлу" });

                newFileUrl = uploadResult.Url;
                urlChanged = true;
            }
            else if (!string.IsNullOrEmpty(dto.LinkUrl))
            {
                newFileUrl = dto.LinkUrl;
                urlChanged = true;
            }

            if (urlChanged)
            {
                var oldPublicId = ExtractPublicIdFromUrl(existingMaterial.FileUrl);
                if (!string.IsNullOrEmpty(oldPublicId))
                {
                    try
                    {
                        await _cloudinaryRepository.DeleteAsync(oldPublicId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete old file from Cloudinary: {ex.Message}");
                    }
                }
            }

            var updated = await _repository.UpdateAsync(id, dto, newFileUrl);

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("subject/{subjectId:guid}")]
        public async Task<ActionResult<IEnumerable<MaterialDto>>> GetBySubject(Guid subjectId)
        {
            var materials = await _repository.GetBySubjectIdAsync(subjectId);

            return Ok(materials);
        }

        private string? ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            try
            {
                var uploadIndex = url.LastIndexOf("/upload/");
                if (uploadIndex == -1) return null;

                var pathAfterUpload = url.Substring(uploadIndex + 8);

                if (pathAfterUpload.StartsWith("v") && pathAfterUpload.Contains("/"))
                {
                    pathAfterUpload = pathAfterUpload.Substring(pathAfterUpload.IndexOf("/") + 1);
                }

                var lastDotIndex = pathAfterUpload.LastIndexOf(".");
                if (lastDotIndex != -1)
                {
                    pathAfterUpload = pathAfterUpload.Substring(0, lastDotIndex);
                }

                return pathAfterUpload;
            }
            catch
            {
                return null;
            }
        }
    }
}
