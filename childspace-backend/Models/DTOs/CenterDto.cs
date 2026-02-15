using childspace_backend.Models.Enums;

namespace childspace_backend.Models.DTOs
{
    public class CenterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public SubscriptionStatus SubscriptionStatus { get; set; }

        public List<UserDto> Users { get; set; }
        public List<ChildDto> Children { get; set; }
        public List<GroupDto> Groups { get; set; }
        public List<TrialRequestDto> TrialRequests { get; set; }
        public List<SubjectDto> Subjects { get; set; }
    }
}
