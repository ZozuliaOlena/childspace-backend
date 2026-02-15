using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using childspace_backend.Data;
using childspace_backend.Models;
using childspace_backend.Utility;

namespace childspace_backend.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ChildSpaceDbContext _dbContext;

        public DbInitializer(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ChildSpaceDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public void Initialize()
        {
            if (_dbContext.Database.GetPendingMigrations().Any())
                _dbContext.Database.Migrate();

            string[] roles = {
                StaticDetail.Role_SuperAdmin,
                StaticDetail.Role_CenterAdmin,
                StaticDetail.Role_Teacher,
                StaticDetail.Role_Parent
            };

            foreach (var role in roles)
            {
                if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole<Guid>(role)).GetAwaiter().GetResult();
                }
            }

            var user = _userManager.FindByEmailAsync("superadmin@childspace.com")
                .GetAwaiter().GetResult();

            if (user == null)
            {
                var superAdmin = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "superadmin@childspace.com",
                    UserName = "superadmin@childspace.com",
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                var result = _userManager.CreateAsync(superAdmin, "SuperAdmin123!")
                    .GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(superAdmin, StaticDetail.Role_SuperAdmin)
                        .GetAwaiter().GetResult();
                }
                else
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

    }
}
