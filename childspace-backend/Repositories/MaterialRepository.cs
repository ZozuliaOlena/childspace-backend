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

        public async Task<IEnumerable<MaterialDto>> GetAllAsync(Guid? centerId = null, Guid? subjectId = null, Guid? groupId = null)
        {
            var query = _context.Materials
                .Include(m => m.Subject)
                .Include(m => m.Teacher)
                .Include(m => m.Group)
                .AsQueryable();

            if (centerId.HasValue)
            {
                query = query.Where(m => m.CenterId == centerId.Value);
            }

            if (subjectId.HasValue)
            {
                query = query.Where(m => m.SubjectId == subjectId.Value);
            }

            if (groupId.HasValue)
            {
                query = query.Where(m => m.GroupId == groupId.Value);
            }

            var materials = await query.ToListAsync();

            return _mapper.Map<List<MaterialDto>>(materials);
        }

        public async Task<MaterialDto?> GetByIdAsync(Guid id)
        {
            var material = await _context.Materials
                .Include(m => m.Subject)
                .Include(m => m.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
                return null;

            var dto = _mapper.Map<MaterialDto>(material);

            if (material.Teacher != null)
            {
                dto.TeacherName = $"{material.Teacher.FirstName} {material.Teacher.LastName}";
            }

            return dto;
        }

        public async Task<MaterialDto> CreateAsync(MaterialCreateDto dto, string fileUrl)
        {
            var material = new Material
            {
                Id = Guid.NewGuid(),
                SubjectId = dto.SubjectId,
                TeacherId = dto.TeacherId,
                Title = dto.Title,
                FileUrl = fileUrl,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                CenterId = dto.CenterId
            };

            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(material.Id);
        }

        public async Task<MaterialDto?> UpdateAsync(Guid id, MaterialUpdateDto dto, string fileUrl)
        {
            var material = await _context.Materials.FindAsync(id);

            if (material == null)
                return null;

            material.Title = dto.Title;
            material.FileUrl = fileUrl;
            material.Description = dto.Description;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
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

        public async Task<IEnumerable<MaterialDto>> GetBySubjectIdAsync(Guid subjectId)
        {
            var materials = await _context.Materials
                .Include(m => m.Subject)
                .Include(m => m.Teacher)
                .Where(m => m.SubjectId == subjectId)
                .ToListAsync();

            var dtos = _mapper.Map<List<MaterialDto>>(materials);

            foreach (var dto in dtos)
            {
                var original = materials.First(m => m.Id == dto.Id);

                if (original.Teacher != null)
                {
                    dto.TeacherName = $"{original.Teacher.FirstName} {original.Teacher.LastName}";
                }

                if (original.Subject != null)
                {
                    dto.SubjectName = original.Subject.Name;
                }
            }

            return dtos;
        }
    }
}
