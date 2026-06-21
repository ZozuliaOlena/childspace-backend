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
    public class TrialRequestController : BaseController 
    {
        private readonly ITrialRequestRepository _repository;

        public TrialRequestController(
            ITrialRequestRepository repository,
            UserManager<User> userManager) : base(userManager)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<IEnumerable<TrialRequestDto>>> GetAll([FromQuery] Guid? centerId)
        {
            Guid? filterCenterId = centerId;

            if (!User.IsInRole(StaticDetail.Role_SuperAdmin))
            {
                var user = await GetCurrentUserAsync();
                if (user == null || user.CenterId == null)
                    return Forbid();

                filterCenterId = user.CenterId;
            }

            var requests = await _repository.GetAllAsync(filterCenterId);
            return Ok(requests);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<TrialRequestDto>> GetById(Guid id)
        {
            var request = await _repository.GetByIdAsync(id);

            if (request == null)
                return NotFound();

            if (!await CheckCenterPermissionsAsync(request.CenterId))
                return StatusCode(403, new { message = "У вас немає доступу до заявок цього центру." });

            return Ok(request);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<TrialRequestDto>> Create(TrialRequestCreateDto dto)
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
        public async Task<ActionResult<TrialRequestDto>> Update(Guid id, TrialRequestUpdateDto dto)
        {
            var existingRequest = await _repository.GetByIdAsync(id);
            if (existingRequest == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(existingRequest.CenterId))
                return StatusCode(403, new { message = "У вас немає доступу до цієї заявки." });

            var updated = await _repository.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var existingRequest = await _repository.GetByIdAsync(id);
            if (existingRequest == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(existingRequest.CenterId))
                return StatusCode(403, new { message = "У вас немає доступу до цієї заявки." });

            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
