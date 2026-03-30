using AutoMapper;
using AutoMapper.QueryableExtensions;
using childspace_backend.Data;
using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using childspace_backend.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserRepository(
            UserManager<User> userManager,
            ChildSpaceDbContext context,
            IMapper mapper,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(Guid? centerId = null)
        {
            var query = _context.Users
                .Include(u => u.Center)
                .Include(u => u.Children)
                .Include(u => u.TeachingGroups)
                .AsQueryable();

            if (centerId.HasValue)
            {
                query = query.Where(u => u.CenterId == centerId.Value);
            }

            var users = await query.ToListAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);

            foreach (var dto in userDtos)
            {
                dto.Roles = await (from userRole in _context.UserRoles
                                   join role in _context.Roles on userRole.RoleId equals role.Id
                                   where userRole.UserId == dto.Id
                                   select role.Name).ToListAsync();
            }

            return userDtos;
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Center)
                .Include(u => u.Children)
                .Include(u => u.TeachingGroups)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(UserCreateDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CenterId = dto.CenterId
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            var validRoles = new List<string>
            {
                StaticDetail.Role_SuperAdmin,
                StaticDetail.Role_CenterAdmin,
                StaticDetail.Role_Teacher,
                StaticDetail.Role_Parent
            };

            var assignedRoles = new List<string>();

            if (dto.Roles != null && dto.Roles.Any())
            {
                var rolesToAdd = dto.Roles.Where(r => validRoles.Contains(r)).ToList();

                if (rolesToAdd.Any())
                {
                    var roleResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception("User created but no roles assigned: " +
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                    assignedRoles = rolesToAdd;
                }
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = assignedRoles;

            if (user.CenterId.HasValue)
            {
                var center = await _context.Centers
                    .AsNoTracking() 
                    .FirstOrDefaultAsync(c => c.Id == user.CenterId.Value);

                if (center != null)
                {
                    userDto.CenterName = center.Name;
                }
            }

            return userDto;
        }

        public async Task<UserDto> UpdateAsync(Guid id, UserUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return null;

            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.CenterId = dto.CenterId;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<IList<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> AddToRolesAsync(User user, IEnumerable<string> roles)
        {
            return await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<string> roles)
        {
            return await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task<IdentityResult> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (dto.NewPassword != dto.ConfirmNewPassword)
                return IdentityResult.Failed(new IdentityError { Description = "New password and confirmation do not match" });

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            return result;
        }
    }
}
