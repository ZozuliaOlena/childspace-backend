using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CenterController : ControllerBase
    {
        private readonly ICenterRepository _repository;

        public CenterController(ICenterRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var centers = await _repository.GetAllAsync();
            return Ok(centers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var center = await _repository.GetByIdAsync(id);
            if (center == null)
                return NotFound(new { message = "Center not found" });

            return Ok(center);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CenterCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var center = await _repository.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = center.Id }, center);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CenterUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var center = await _repository.UpdateAsync(id, dto);
            if (center == null)
                return NotFound(new { message = "Center not found" });

            return Ok(center);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _repository.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Center not found" });

            return Ok(new { message = "Center deleted successfully" });
        }
    }
}
