using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<UserChat> UserChats { get; set; }
    }
}
