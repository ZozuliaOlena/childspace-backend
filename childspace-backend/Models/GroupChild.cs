using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class GroupChild
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public Guid ChildId { get; set; }
        [ForeignKey("ChildId")]
        public virtual Child Child { get; set; }
    }
}
