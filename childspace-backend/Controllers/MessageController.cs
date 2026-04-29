using childspace_backend.Hubs;
using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : BaseController
    {
        private readonly IMessageRepository _repository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatRepository _chatRepository;
        private readonly IFirebaseNotificationService _firebaseService;

        public MessageController(
            IMessageRepository repository,
            IHubContext<ChatHub> hubContext,
            IChatRepository chatRepository,
            IFirebaseNotificationService firebaseService,
            UserManager<User> userManager) : base(userManager)
        {
            _repository = repository;
            _hubContext = hubContext;
            _chatRepository = chatRepository;
            _firebaseService = firebaseService;
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
        public async Task<ActionResult<IEnumerable<ChatMessageResponseDto>>> GetChatMessages(
            Guid chatId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var messages = await _repository.GetMessagesByChatIdAsync(chatId, page, pageSize);

            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<ActionResult<ChatMessageResponseDto>> SendMessage([FromBody] SendMessageDto dto)
        {
            var sender = await GetCurrentUserAsync();
            if (sender == null) return Unauthorized();

            try
            {
                var message = await _repository.SendMessageAsync(sender.Id, dto);

                await _hubContext.Clients.Group(dto.ChatId.ToString()).SendAsync("ReceiveMessage", message);

                var participants = (await _chatRepository.GetChatParticipantsAsync(dto.ChatId)).ToList();

                foreach (var participant in participants)
                {
                    if (participant.Id != sender.Id)
                    {
                        var receiver = await _userManager.FindByIdAsync(participant.Id.ToString());

                        if (receiver != null && !string.IsNullOrEmpty(receiver.FcmToken))
                        {
                            string title = $"Нове повідомлення від {sender.FirstName}";
                            await _firebaseService.SendNotificationAsync(receiver.FcmToken, title, dto.Content);
                        }
                    }
                }

                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
