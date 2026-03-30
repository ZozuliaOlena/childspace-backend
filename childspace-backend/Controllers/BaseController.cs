using childspace_backend.Models;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace childspace_backend.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly UserManager<User> _userManager;

        public BaseController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected async Task<bool> CheckPermissionsAsync(Guid? targetCenterId)
        {
            if (User.IsInRole(StaticDetail.Role_SuperAdmin))
                return true;

            if (!targetCenterId.HasValue)
                return false;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.CenterId == null)
                return false;

            return user.CenterId == targetCenterId.Value;
        }
    }
}