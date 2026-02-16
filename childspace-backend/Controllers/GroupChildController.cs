using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupChildController : ControllerBase
    {
        private readonly IGroupChildRepository _repository;

        public GroupChildController(IGroupChildRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupChildDto>>> GetAll()
        {
            var groupChildren = await _repository.GetAllAsync();
            return Ok(groupChildren);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GroupChildDto>> GetById(Guid id)
        {
            var groupChild = await _repository.GetByIdAsync(id);

            if (groupChild == null)
                return NotFound();

            return Ok(groupChild);
        }

        [HttpPost]
        public async Task<ActionResult<GroupChildDto>> Create(GroupChildCreateDto dto)
        {
            var created = await _repository.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<GroupChildDto>> Update(Guid id, GroupChildUpdateDto dto)
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
