using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class Child
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual Center Center { get; set; }

        public Guid ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual User Parent { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Notes { get; set; }

        public virtual ICollection<GroupChild> GroupChildren { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
    }
}
