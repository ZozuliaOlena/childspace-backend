using childspace_backend.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace childspace_backend.Models
{
    public class Center
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public SubscriptionStatus SubscriptionStatus { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Child> Children { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<TrialRequest> TrialRequests { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
