using System.Text.Json.Serialization;

namespace childspace_backend.Models.DTOs
{
    public class ChildDto
    {
        public Guid Id { get; set; }

        public Guid CenterId { get; set; }
        [JsonIgnore]
        public CenterDto Center { get; set; }

        public Guid ParentId { get; set; }
        [JsonIgnore]
        public UserDto Parent { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Notes { get; set; }

        [JsonIgnore]
        public List<GroupChildDto> Groups { get; set; } = new List<GroupChildDto>();
        [JsonIgnore]
        public List<AttendanceDto> Attendances { get; set; } = new List<AttendanceDto>();
    }

}
