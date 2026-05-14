using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupController : BaseController
    {
        private readonly IGroupRepository _repository;

        public GroupController(IGroupRepository repository, UserManager<User> userManager) : base(userManager)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetAll()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            Guid? filterTeacherId = null;
            Guid? filterCenterId = null;

            if (User.IsInRole(StaticDetail.Role_Teacher))
            {
                filterTeacherId = user.Id;
            }
            else if (User.IsInRole(StaticDetail.Role_CenterAdmin))
            {
                filterCenterId = user.CenterId;
            }
            else if (User.IsInRole(StaticDetail.Role_Parent))
            {
                return Forbid();
            }

            var groups = await _repository.GetAllAsync(filterTeacherId, filterCenterId);
            return Ok(groups);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GroupDto>> GetById(Guid id)
        {
            var group = await _repository.GetByIdAsync(id);

            if (group == null)
                return NotFound();

            return Ok(group);
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<GroupDto>> Create(GroupCreateDto dto)
        {
            var user = await GetCurrentUserAsync();
            if (user?.CenterId != null)
            {
                dto.CenterId = user.CenterId.Value;
            }

            var created = await _repository.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<GroupDto>> Update(Guid id, GroupUpdateDto dto)
        {
            var updated = await _repository.UpdateAsync(id, dto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{id:guid}/children")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin},{StaticDetail.Role_Teacher}")]
        public async Task<ActionResult<IEnumerable<ChildDto>>> GetGroupChildren(Guid id)
        {
            var children = await _repository.GetChildrenByGroupIdAsync(id);
            return Ok(children);
        }
    }
}
