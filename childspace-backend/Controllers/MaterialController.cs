using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class MaterialController : BaseController 
    {
        private readonly IMaterialRepository _repository;
        private readonly ICloudinaryRepository _cloudinaryRepository;

        public MaterialController(
            IMaterialRepository repository,
            ICloudinaryRepository cloudinaryRepository,
            UserManager<User> userManager) : base(userManager)
        {
            _repository = repository;
            _cloudinaryRepository = cloudinaryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAll([FromQuery] Guid? subjectId)
        {
            Guid? filterCenterId = null;

            if (!User.IsInRole(StaticDetail.Role_SuperAdmin))
            {
                var user = await GetCurrentUserAsync();
                if (user == null || user.CenterId == null) return Forbid();

                filterCenterId = user.CenterId;
            }

            var materials = await _repository.GetAllAsync(filterCenterId, subjectId);
            return Ok(materials);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MaterialDto>> GetById(Guid id)
        {
            var material = await _repository.GetByIdAsync(id);
            if (material == null) return NotFound();
            return Ok(material);
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin},{StaticDetail.Role_Teacher}")]
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

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin},{StaticDetail.Role_Teacher}")]
        [RequestSizeLimit(104857600)]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<ActionResult<MaterialDto>> Update(Guid id, [FromForm] MaterialUpdateDto dto)
        {
            var existingMaterial = await _repository.GetByIdAsync(id);
            if (existingMaterial == null) return NotFound();

            if (!IsOwner(existingMaterial.TeacherId) && !User.IsInRole(StaticDetail.Role_CenterAdmin))
            {
                return Forbid();
            }

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
                var oldPublicId = CloudinaryHelper.ExtractPublicIdFromUrl(existingMaterial.FileUrl);
                if (!string.IsNullOrEmpty(oldPublicId))
                {
                    try { await _cloudinaryRepository.DeleteAsync(oldPublicId); }
                    catch (Exception ex) { Console.WriteLine($"Failed to delete old file from Cloudinary: {ex.Message}"); }
                }
            }

            var updated = await _repository.UpdateAsync(id, dto, newFileUrl);
            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin},{StaticDetail.Role_Teacher}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var existingMaterial = await _repository.GetByIdAsync(id);
            if (existingMaterial == null) return NotFound();

            if (!IsOwner(existingMaterial.TeacherId) && !User.IsInRole(StaticDetail.Role_CenterAdmin))
            {
                return Forbid();
            }

            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return NotFound();

            var publicId = CloudinaryHelper.ExtractPublicIdFromUrl(existingMaterial.FileUrl);
            if (!string.IsNullOrEmpty(publicId))
            {
                try { await _cloudinaryRepository.DeleteAsync(publicId); }
                catch (Exception ex) { Console.WriteLine($"Failed to delete file from Cloudinary: {ex.Message}"); }
            }

            return NoContent();
        }

        [HttpGet("subject/{subjectId:guid}")]
        public async Task<ActionResult<IEnumerable<MaterialDto>>> GetBySubject(Guid subjectId)
        {
            var materials = await _repository.GetBySubjectIdAsync(subjectId);
            return Ok(materials);
        }
    }
}
