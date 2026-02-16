using AutoMapper;
using childspace_backend.Data;
using childspace_backend.Models.DTOs;
using childspace_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace childspace_backend.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly ChildSpaceDbContext _context;
        private readonly IMapper _mapper;

        public MaterialRepository(ChildSpaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaterialDto>> GetAllAsync()
        {
            var materials = await _context.Materials
                .Include(m => m.Group)
                .Include(m => m.Teacher)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MaterialDto>>(materials);
        }

        public async Task<MaterialDto?> GetByIdAsync(Guid id)
        {
            var material = await _context.Materials
                .Include(m => m.Group)
                .Include(m => m.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
                return null;

            return _mapper.Map<MaterialDto>(material);
        }

        public async Task<MaterialDto> CreateAsync(MaterialCreateDto dto)
        {
            var material = new Material
            {
                Id = Guid.NewGuid(),
                GroupId = dto.GroupId,
                TeacherId = dto.TeacherId,
                Title = dto.Title,
                FileUrl = dto.FileUrl,
                Description = dto.Description,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow
            };

            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            return _mapper.Map<MaterialDto>(material);
        }

        public async Task<MaterialDto?> UpdateAsync(Guid id, MaterialUpdateDto dto)
        {
            var material = await _context.Materials.FindAsync(id);

            if (material == null)
                return null;

            material.Title = dto.Title;
            material.FileUrl = dto.FileUrl;
            material.Description = dto.Description;
            material.Type = dto.Type;

            await _context.SaveChangesAsync();

            return _mapper.Map<MaterialDto>(material);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var material = await _context.Materials.FindAsync(id);

            if (material == null)
                return false;

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
