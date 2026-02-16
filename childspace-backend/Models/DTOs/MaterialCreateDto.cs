using childspace_backend.Models.Enums;

namespace childspace_backend.Models.DTOs
{
    public class MaterialCreateDto
    {
        public Guid GroupId { get; set; }

        public Guid TeacherId { get; set; }

        public string Title { get; set; }

        public string FileUrl { get; set; }

        public string? Description { get; set; }

        public MaterialType Type { get; set; }
    }
}
