using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class Subject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
        public string? Description { get; set; }

        public Guid CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual Center Center { get; set; }
    }
}
