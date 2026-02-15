using childspace_backend.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models.DTOs
{
    public class CenterCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Active;
    }
}
