using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class TrialRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual Center Center { get; set; }

        public string ParentName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ChildName { get; set; }
        public Guid ChildAge { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
