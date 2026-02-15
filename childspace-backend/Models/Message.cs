using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public int UserChatId { get; set; }
        [ForeignKey("UserChatId")]
        public virtual UserChat UserChat { get; set; }

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
