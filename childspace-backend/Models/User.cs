using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace childspace_backend.Models
{
    public class User : IdentityUser<Guid>
    {
        public Guid? CenterId { get; set; }
        [ForeignKey("CenterId")]
        public virtual Center Center { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Child> Children { get; set; }
        public virtual ICollection<Group> TeachingGroups { get; set; }
        public virtual ICollection<UserChat> UserChats { get; set; }
    }
}
