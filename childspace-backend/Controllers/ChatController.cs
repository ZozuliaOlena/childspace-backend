using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _repository;

        public ChatController(IChatRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetAll()
        {
            var chats = await _repository.GetAllAsync();
            return Ok(chats);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ChatDto>> GetById(Guid id)
        {
            var chat = await _repository.GetByIdAsync(id);

            if (chat == null)
                return NotFound();

            return Ok(chat);
        }

        [HttpPost]
        public async Task<ActionResult<ChatDto>> Create(ChatCreateDto dto)
        {
            var created = await _repository.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ChatDto>> Update(Guid id, ChatUpdateDto dto)
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

        [HttpGet("{id:guid}/participants")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetParticipants(Guid id)
        {
            try
            {
                var participants = await _repository.GetChatParticipantsAsync(id);
                return Ok(participants);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
