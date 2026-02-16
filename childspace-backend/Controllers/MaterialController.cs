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

        public MaterialController(IMaterialRepository repository)
        {
            _repository = repository;
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
        public async Task<ActionResult<MaterialDto>> Create(MaterialCreateDto dto)
        {
            var created = await _repository.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MaterialDto>> Update(Guid id, MaterialUpdateDto dto)
        {
            var updated = await _repository.UpdateAsync(id, dto);

            if (updated == null)
                return NotFound();

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
    }
}
