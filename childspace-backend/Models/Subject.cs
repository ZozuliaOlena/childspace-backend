using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }

        public int CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual Center Center { get; set; }
    }
}
