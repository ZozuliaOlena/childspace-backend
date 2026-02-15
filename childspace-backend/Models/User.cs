using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace childspace_backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public int? CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual Center Center { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Child> Children { get; set; }
        public virtual ICollection<Group> TeachingGroups { get; set; }
        public virtual ICollection<UserChat> UserChats { get; set; }
    }
}
