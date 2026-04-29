using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using childspace_backend.Repositories;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository repository, UserManager<User> userManager)
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

                if (filterCenterId == null) return Forbid();
            }

            var users = await _repository.GetAllAsync(filterCenterId);
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var targetUser = await _repository.GetByIdAsync(id);
            if (targetUser == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(targetUser.CenterId))
                return Forbid();

            return Ok(targetUser);
        }

        [HttpPost]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (!await CheckCenterPermissionsAsync(dto.CenterId))
                return Forbid();

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
            var existingUser = await _repository.GetByIdAsync(id);
            if (existingUser == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(existingUser.CenterId))
                return Forbid();

            if (!await CheckCenterPermissionsAsync(dto.CenterId))
                return Forbid();

            var user = await _repository.UpdateAsync(id, dto);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var targetUser = await _userManager.FindByIdAsync(id.ToString());
            if (targetUser == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(targetUser.CenterId))
                return Forbid();

            var result = await _repository.DeleteAsync(id);
            return Ok(new { message = "User deleted successfully" });
        }


        [HttpGet("{id:guid}/roles")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> GetUserRoles(Guid id)
        {
            var userEntity = await _userManager.FindByIdAsync(id.ToString());
            if (userEntity == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(userEntity.CenterId)) return Forbid();

            var roles = await _repository.GetUserRolesAsync(userEntity);
            return Ok(roles);
        }

        [HttpPost("{id:guid}/roles")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> AddRoles(Guid id, [FromBody] string[] roles)
        {
            var userEntity = await _userManager.FindByIdAsync(id.ToString());
            if (userEntity == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(userEntity.CenterId)) return Forbid();

            var result = await _repository.AddToRolesAsync(userEntity, roles);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = "Roles added successfully" });
        }

        [HttpDelete("{id:guid}/roles")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<IActionResult> RemoveRoles(Guid id, [FromBody] string[] roles)
        {
            var userEntity = await _userManager.FindByIdAsync(id.ToString());
            if (userEntity == null) return NotFound();

            if (!await CheckCenterPermissionsAsync(userEntity.CenterId)) return Forbid();

            var result = await _repository.RemoveFromRolesAsync(userEntity, roles);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = "Roles removed successfully" });
        }

        [HttpPost("{id:guid}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
        {
            if (!IsOwner(id)) return Forbid();

            var result = await _repository.ChangePasswordAsync(id, dto);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
                return NotFound(new { message = "User not found in database." });

            var userDto = await _repository.GetByIdAsync(user.Id);
            return Ok(userDto);
        }

        [HttpPost("fcm-token")]
        public async Task<IActionResult> UpdateFcmToken([FromBody] string fcmToken)
        {
            if (string.IsNullOrEmpty(fcmToken))
                return BadRequest(new { message = "Токен не може бути порожнім" });

            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            user.FcmToken = fcmToken;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new { message = "Помилка при збереженні токена" });

            return Ok(new { message = "FCM токен успішно оновлено" });
        }

        [HttpGet("available-for-chat")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAvailableUsersForChat()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdString, out var userId))
                    return Unauthorized(new { message = "Невірний токен" });

                bool isSuperAdmin = User.IsInRole(StaticDetail.Role_SuperAdmin);

                var users = await _repository.GetUsersForChatAsync(userId, isSuperAdmin);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("parents")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetParents()
        {
            Guid? filterCenterId = null;

            if (!User.IsInRole(StaticDetail.Role_SuperAdmin))
            {
                var user = await GetCurrentUserAsync();
                filterCenterId = user?.CenterId;
            }

            var parents = await _repository.GetUsersByRoleAsync(StaticDetail.Role_Parent, filterCenterId);
            return Ok(parents);
        }

        [HttpGet("teachers")]
        [Authorize(Roles = $"{StaticDetail.Role_SuperAdmin},{StaticDetail.Role_CenterAdmin}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetTeachers()
        {
            Guid? filterCenterId = null;

            if (!User.IsInRole(StaticDetail.Role_SuperAdmin))
            {
                var user = await GetCurrentUserAsync();
                filterCenterId = user?.CenterId;
            }

            var teachers = await _repository.GetUsersByRoleAsync(StaticDetail.Role_Teacher, filterCenterId);
            return Ok(teachers);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetMobileProfile()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
                return NotFound(new { message = "User not found in database." });

            var userWithDetails = await _userManager.Users
                .Include(u => u.Children)
                    .ThenInclude(c => c.GroupChildren)
                        .ThenInclude(gc => gc.Group)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithDetails == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(userWithDetails);
            var mainRole = roles.FirstOrDefault() ?? "Користувач";

            var profileDto = new UserProfileDto
            {
                FirstName = userWithDetails.FirstName,
                LastName = userWithDetails.LastName,
                Email = userWithDetails.Email,
                PhoneNumber = userWithDetails.PhoneNumber,
                Role = mainRole,
                Children = userWithDetails.Children.Select(child => new ChildProfileDto
                {
                    Name = $"{child.FirstName} {child.LastName}",
                    Age = CalculateAge(child.BirthDate),
                    GroupNames = child.GroupChildren.Select(gc => gc.Group.Name).ToList()
                }).ToList()
            };

            return Ok(profileDto);
        }

        private static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
