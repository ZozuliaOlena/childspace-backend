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
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    _dbContext.Database.Migrate();
                }
            }
            catch
            {
            }

            if (_roleManager.RoleExistsAsync(StaticDetail.Role_SuperAdmin)
                .GetAwaiter().GetResult())
                return;

            _roleManager.CreateAsync(new IdentityRole<Guid>(StaticDetail.Role_SuperAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole<Guid>(StaticDetail.Role_CenterAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole<Guid>(StaticDetail.Role_Teacher)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole<Guid>(StaticDetail.Role_Parent)).GetAwaiter().GetResult();

            var superAdmin = new User
            {
                Id = Guid.NewGuid(),
                Email = "superadmin@childspace.com",
                FirstName = "Super",
                LastName = "Admin"
            };

            _userManager.CreateAsync(superAdmin, "SuperAdmin123!")
                .GetAwaiter().GetResult();

            _userManager.AddToRoleAsync(superAdmin, StaticDetail.Role_SuperAdmin)
                .GetAwaiter().GetResult();
        }
    }
}
