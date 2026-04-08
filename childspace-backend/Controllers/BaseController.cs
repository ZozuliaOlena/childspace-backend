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

        protected async Task<User?> GetCurrentUserAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;

            return await _userManager.FindByIdAsync(userId);
        }

        protected async Task<bool> CheckCenterPermissionsAsync(Guid? targetCenterId)
        {
            if (User.IsInRole(StaticDetail.Role_SuperAdmin))
                return true;

            if (!targetCenterId.HasValue)
                return false;

            var user = await GetCurrentUserAsync();
            if (user == null || user.CenterId == null)
                return false;

            if (User.IsInRole(StaticDetail.Role_CenterAdmin) || User.IsInRole(StaticDetail.Role_Teacher))
            {
                return user.CenterId == targetCenterId.Value;
            }

            return false;
        }

        protected bool IsOwner(Guid resourceOwnerId)
        {
            if (User.IsInRole(StaticDetail.Role_SuperAdmin))
                return true;

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return false;

            return currentUserId == resourceOwnerId.ToString();
        }
    }
}