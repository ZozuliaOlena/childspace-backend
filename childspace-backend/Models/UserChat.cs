using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class UserChat
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int ChatId { get; set; }
        [ForeignKey("ChatId")]
        public virtual Chat Chat { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
