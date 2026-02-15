using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class Chat
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<UserChat> UserChats { get; set; }
    }
}
