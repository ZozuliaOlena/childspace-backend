using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
    public class UserChatController : ControllerBase
    {
        private readonly IUserChatRepository _repository;

        public UserChatController(IUserChatRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserChatDto>>> GetAll()
        {
            var userChats = await _repository.GetAllAsync();
            return Ok(userChats);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserChatDto>> GetById(Guid id)
        {
            var userChat = await _repository.GetByIdAsync(id);

            if (userChat == null)
                return NotFound();

            return Ok(userChat);
        }

        [HttpPost]
        public async Task<ActionResult<UserChatDto>> Create(UserChatCreateDto dto)
        {
            var created = await _repository.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserChatDto>> Update(Guid id, UserChatUpdateDto dto)
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
