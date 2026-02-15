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
            var child = new Child
            {
                Id = Guid.NewGuid(),
                CenterId = dto.CenterId,
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
