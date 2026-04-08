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
    public class ChildController : BaseController
    {
        private readonly IChildRepository _childRepository;

        public ChildController(
            IChildRepository childRepository,
            UserManager<User> userManager) : base(userManager)
        {
            _childRepository = childRepository;
        }

        [HttpGet]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin},{StaticDetail.Role_Teacher}")]
        public async Task<IActionResult> GetAll()
        {
            var children = await _childRepository.GetAllAsync();
            return Ok(children);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin},{StaticDetail.Role_Teacher},{StaticDetail.Role_Parent}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var child = await _childRepository.GetByIdAsync(id);
            if (child == null) return NotFound(new { message = "Child not found" });

            if (User.IsInRole(StaticDetail.Role_Parent))
            {
                if (!IsOwner(child.ParentId))
                    return Forbid(); 
            }
            else
            {
                if (!await CheckCenterPermissionsAsync(child.CenterId))
                    return Forbid();
            }

            return Ok(child);
        }

        [HttpGet("parent/{parentId}")]
        [Authorize]
        public async Task<IActionResult> GetByParentId(Guid parentId)
        {
            var children = await _childRepository.GetByParentIdAsync(parentId);
            return Ok(children);
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Create([FromBody] CreateChildDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid data" });

            try
            {
                var createdChild = await _childRepository.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdChild.Id },
                    createdChild
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChildDto dto)
        {
            var updatedChild = await _childRepository.UpdateAsync(id, dto);

            if (updatedChild == null)
                return NotFound(new { message = "Child not found" });

            return Ok(updatedChild);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _childRepository.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Child not found" });

            return Ok(new { message = "Child deleted successfully" });
        }
    }
}
