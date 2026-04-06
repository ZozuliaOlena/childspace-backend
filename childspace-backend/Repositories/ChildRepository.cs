using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using childspace_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class ChildRepository : IChildRepository
    {
        private readonly ChildSpaceDbContext _context;

        public ChildRepository(ChildSpaceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChildDto>> GetAllAsync()
        {
            return await _context.Children
                .Select(child => new ChildDto
                {
                    Id = child.Id,
                    CenterId = child.CenterId,
                    ParentId = child.ParentId,
                    FirstName = child.FirstName,
                    LastName = child.LastName,
                    BirthDate = child.BirthDate,
                    Notes = child.Notes
                })
                .ToListAsync();
        }

        public async Task<ChildDto?> GetByIdAsync(Guid id)
        {
            var child = await _context.Children.FindAsync(id);

            if (child == null) return null;

            return new ChildDto
            {
                Id = child.Id,
                CenterId = child.CenterId,
                ParentId = child.ParentId,
                FirstName = child.FirstName,
                LastName = child.LastName,
                BirthDate = child.BirthDate,
                Notes = child.Notes
            };
        }

        public async Task<IEnumerable<ChildDto>> GetByParentIdAsync(Guid parentId)
        {
            return await _context.Children
                .Where(c => c.ParentId == parentId)
                .Select(child => new ChildDto
                {
                    Id = child.Id,
                    CenterId = child.CenterId,
                    ParentId = child.ParentId,
                    FirstName = child.FirstName,
                    LastName = child.LastName,
                    BirthDate = child.BirthDate,
                    Notes = child.Notes
                })
                .ToListAsync();
        }

        public async Task<ChildDto?> CreateAsync(CreateChildDto dto)
        {
            var parent = await _context.Users.FindAsync(dto.ParentId);

            if (parent == null)
                throw new Exception("Parent not found in database.");

            if (parent.CenterId == null)
                throw new Exception("The parent has no center specified. Unable to link the child.");

            var child = new Child
            {
                Id = Guid.NewGuid(),
                CenterId = parent.CenterId.Value,
                ParentId = dto.ParentId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                Notes = dto.Notes
            };

            _context.Children.Add(child);
            await _context.SaveChangesAsync();

            return new ChildDto
            {
                Id = child.Id,
                CenterId = child.CenterId,
                ParentId = child.ParentId,
                FirstName = child.FirstName,
                LastName = child.LastName,
                BirthDate = child.BirthDate,
                Notes = child.Notes
            };
        }

        public async Task<ChildDto?> UpdateAsync(Guid id, UpdateChildDto dto)
        {
            var child = await _context.Children.FindAsync(id);

            if (child == null) return null;

            child.FirstName = dto.FirstName;
            child.LastName = dto.LastName;
            child.BirthDate = dto.BirthDate;
            child.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return new ChildDto
            {
                Id = child.Id,
                CenterId = child.CenterId,
                ParentId = child.ParentId,
                FirstName = child.FirstName,
                LastName = child.LastName,
                BirthDate = child.BirthDate,
                Notes = child.Notes
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var child = await _context.Children.FindAsync(id);

            if (child == null) return false;

            _context.Children.Remove(child);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
