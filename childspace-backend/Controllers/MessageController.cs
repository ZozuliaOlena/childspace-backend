using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _repository;

        public MessageController(IMessageRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetAll()
        {
            var messages = await _repository.GetAllAsync();
            return Ok(messages);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MessageDto>> GetById(Guid id)
        {
            var message = await _repository.GetByIdAsync(id);

            if (message == null)
                return NotFound();

            return Ok(message);
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> Create(MessageCreateDto dto)
        {
            var created = await _repository.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MessageDto>> Update(Guid id, MessageUpdateDto dto)
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
