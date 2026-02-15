using System.Text.Json.Serialization;

namespace childspace_backend.Models.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid? CenterId { get; set; }
        public string CenterName { get; set; }

        [JsonIgnore]
        public List<ChildDto> Children { get; set; }
        [JsonIgnore]
        public List<GroupDto> TeachingGroups { get; set; }
    }
}
