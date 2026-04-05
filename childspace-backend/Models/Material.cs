using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using childspace_backend.Models.Enums;

namespace childspace_backend.Models
{
    public class Material
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }

        public Guid TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public virtual User Teacher { get; set; }

        public string Title { get; set; }
        public string FileUrl { get; set; }
        public string? Description { get; set; }
        public MaterialType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
