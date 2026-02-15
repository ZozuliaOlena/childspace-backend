using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChildController : ControllerBase
    {
        private readonly IChildRepository _childRepository;

        public ChildController(IChildRepository childRepository)
        {
            _childRepository = childRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var children = await _childRepository.GetAllAsync();
            return Ok(children);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var child = await _childRepository.GetByIdAsync(id);

            if (child == null)
                return NotFound(new { message = "Child not found" });

            return Ok(child);
        }

        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetByParentId(Guid parentId)
        {
            var children = await _childRepository.GetByParentIdAsync(parentId);
            return Ok(children);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChildDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid data" });

            var createdChild = await _childRepository.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdChild.Id },
                createdChild
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChildDto dto)
        {
            var updatedChild = await _childRepository.UpdateAsync(id, dto);

            if (updatedChild == null)
                return NotFound(new { message = "Child not found" });

            return Ok(updatedChild);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _childRepository.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Child not found" });

            return Ok(new { message = "Child deleted successfully" });
        }
    }
}
