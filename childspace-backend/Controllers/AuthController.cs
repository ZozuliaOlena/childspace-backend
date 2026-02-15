using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace childspace_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid login attempt" });

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return Unauthorized(new { message = "User account is locked out." });
            }

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid login attempt" });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                message = "Logged in successfully",
                userId = user.Id,
                userName = user.UserName,
                email = user.Email,
                roles
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out" });
        }

        [HttpGet("status")]
        public async Task<IActionResult> Status()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Ok(new { isLoggedIn = false });
                }
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new
                {
                    isLoggedIn = true,
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    roles
                });
            }

            return Ok(new { isLoggedIn = false });
        }
    }
}
