using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly UserManager<User> _userManager;

        public UserController(IUserRepository repository, UserManager<User> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _repository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            try
            {
                var user = await _repository.CreateAsync(dto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
        {
            var user = await _repository.UpdateAsync(id, dto);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _repository.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok(new { message = "User deleted successfully" });
        }

        [HttpGet("roles")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _repository.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id:guid}/roles")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> GetUserRoles(Guid id)
        {
            var userEntity = await _userManager.FindByIdAsync(id.ToString());
            if (userEntity == null) return NotFound();

            var roles = await _repository.GetUserRolesAsync(userEntity);
            return Ok(roles);
        }

        [HttpPost("{id:guid}/roles")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> AddRoles(Guid id, [FromBody] string[] roles)
        {
            var userEntity = await _userManager.FindByIdAsync(id.ToString());
            if (userEntity == null) return NotFound();

            var result = await _repository.AddToRolesAsync(userEntity, roles);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Roles added successfully" });
        }

        [HttpDelete("{id:guid}/roles")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> RemoveRoles(Guid id, [FromBody] string[] roles)
        {
            var userEntity = await _userManager.FindByIdAsync(id.ToString());
            if (userEntity == null) return NotFound();

            var result = await _repository.RemoveFromRolesAsync(userEntity, roles);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Roles removed successfully" });
        }

        [HttpPost("{id:guid}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
        {
            var result = await _repository.ChangePasswordAsync(id, dto);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
                return Unauthorized(new
                {
                    message = "Token is valid, but User ID claim is missing.",
                    availableClaims = claims
                });
            }

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest(new { message = "Invalid User ID format." });
            }

            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found in database." });

            return Ok(user);
        }
    }
}
