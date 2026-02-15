using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class GroupChild
    {
        [Key]
        public int Id { get; set; }

        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public int ChildId { get; set; }
        [ForeignKey("ChildId")]
        public virtual Child Child { get; set; }
    }
}
