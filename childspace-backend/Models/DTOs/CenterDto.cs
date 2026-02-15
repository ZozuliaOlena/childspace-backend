using childspace_backend.Models.Enums;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public List<UserDto> Users { get; set; }
        [JsonIgnore]
        public List<ChildDto> Children { get; set; }
        [JsonIgnore]
        public List<GroupDto> Groups { get; set; }
        [JsonIgnore]
        public List<TrialRequestDto> TrialRequests { get; set; }
        [JsonIgnore]
        public List<SubjectDto> Subjects { get; set; }
    }
}
