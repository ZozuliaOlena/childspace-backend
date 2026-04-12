using childspace_backend.Hubs;
using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _repository;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IMessageRepository repository, IHubContext<ChatHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
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

        [HttpGet("chat/{chatId:guid}")]
        public async Task<ActionResult<IEnumerable<ChatMessageResponseDto>>> GetChatMessages(Guid chatId)
        {
            var messages = await _repository.GetMessagesByChatIdAsync(chatId);
            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<ActionResult<ChatMessageResponseDto>> SendMessage([FromBody] SendMessageDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            try
            {
                var message = await _repository.SendMessageAsync(userId, dto);

                await _hubContext.Clients.Group(dto.ChatId.ToString()).SendAsync("ReceiveMessage", message);

                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
