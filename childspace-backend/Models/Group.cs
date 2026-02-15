using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        public int CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual Center Center { get; set; }

        [Required]
        public string Name { get; set; }

        public int? TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public virtual User Teacher { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<GroupChild> GroupChildren { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Material> Materials { get; set; }
    }
}
