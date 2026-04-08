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
    public class CenterController : BaseController
    {
        private readonly ICenterRepository _repository;

        public CenterController(ICenterRepository repository, UserManager<User> userManager)
            : base(userManager)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> GetAll()
        {
            Guid? filterCenterId = null;

            if (!User.IsInRole(StaticDetail.Role_SuperAdmin))
            {
                var user = await GetCurrentUserAsync();
                filterCenterId = user?.CenterId;

                if (filterCenterId == null)
                    return Forbid();
            }

            var centers = await _repository.GetAllAsync(filterCenterId);
            return Ok(centers);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (!await CheckCenterPermissionsAsync(id))
            {
                return Forbid();
            }

            var center = await _repository.GetByIdAsync(id);
            if (center == null)
                return NotFound(new { message = "Center not found" });

            return Ok(center);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetail.Role_SuperAdmin)]
        public async Task<IActionResult> Create([FromBody] CenterCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var center = await _repository.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = center.Id }, center);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CenterUpdateDto dto)
        {
            if (!await CheckCenterPermissionsAsync(id))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var center = await _repository.UpdateAsync(id, dto);
            if (center == null)
                return NotFound(new { message = "Center not found" });

            return Ok(center);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = StaticDetail.Role_SuperAdmin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _repository.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Center not found" });

            return Ok(new { message = "Center deleted successfully" });
        }
    }
}
