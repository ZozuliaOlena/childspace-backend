using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Utility;
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
        [Authorize] 
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetAll()
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Користувач не авторизований" });
            }

            var chats = await _repository.GetUserChatsAsync(userId);

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
        [Authorize]
        public async Task<ActionResult<ChatDto>> Create(ChatCreateDto dto)
        {
            var created = await _repository.CreateAsync(dto);

            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdString, out var userId))
            {
                await _repository.AddParticipantAsync(created.Id, userId);
            }

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

        [HttpPost("{chatId:guid}/participants/{userId:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> AddParticipant(Guid chatId, Guid userId)
        {
            var result = await _repository.AddParticipantAsync(chatId, userId);

            if (!result)
                return BadRequest(new { message = "Не вдалося додати користувача (чат або користувач не знайдено)" });

            return Ok(new { message = "Користувача успішно додано до чату" });
        }

        [HttpDelete("{chatId:guid}/participants/{userId:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> RemoveParticipant(Guid chatId, Guid userId)
        {
            var result = await _repository.RemoveParticipantAsync(chatId, userId);

            if (!result)
                return NotFound(new { message = "Користувача не знайдено в цьому чаті" });

            return Ok(new { message = "Користувача успішно видалено з чату" });
        }

        [HttpPost("{chatId:guid}/mark-read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(Guid chatId)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var result = await _repository.MarkChatAsReadAsync(chatId, userId);

            if (!result)
                return NotFound(new { message = "Чат не знайдено або ви не є його учасником" });

            return Ok(new { message = "Чат позначено як прочитаний" });
        }

        [HttpGet("has-unread")]
        [Authorize]
        public async Task<IActionResult> CheckForUnreadMessages()
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Користувач не авторизований" });
            }

            bool hasAnyUnread = await _repository.HasUnreadMessagesAsync(userId);

            return Ok(new { hasUnread = hasAnyUnread });
        }
    }
}
