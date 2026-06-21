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
    public class GroupChildController : BaseController 
    {
        private readonly IGroupChildRepository _repository;

        public GroupChildController(
            IGroupChildRepository repository,
            UserManager<User> userManager) : base(userManager)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin},{StaticDetail.Role_Teacher}")]
        public async Task<ActionResult<IEnumerable<GroupChildDto>>> GetAll([FromQuery] Guid? centerId)
        {
            Guid? filterCenterId = centerId;

            if (!User.IsInRole(StaticDetail.Role_SuperAdmin))
            {
                var user = await GetCurrentUserAsync();
                if (user == null || user.CenterId == null) return Forbid();

                filterCenterId = user.CenterId;
            }

            var groupChildren = await _repository.GetAllAsync(filterCenterId);
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
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
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
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<GroupChildDto>> Update(Guid id, GroupChildUpdateDto dto)
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

        [HttpDelete("~/api/Group/{groupId:guid}/child/{childId:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult> DeleteByGroupAndChild(Guid groupId, Guid childId)
        {
            var deleted = await _repository.DeleteByGroupAndChildAsync(groupId, childId);

            if (!deleted)
                return NotFound(new { message = "Ця дитина не перебуває у цій групі." });

            return NoContent();
        }
    }
}
