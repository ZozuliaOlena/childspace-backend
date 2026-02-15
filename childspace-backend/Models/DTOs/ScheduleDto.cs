using System.Text.Json.Serialization;

namespace childspace_backend.Models.DTOs
{
    public class ScheduleDto
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }
        [JsonIgnore]
        public GroupDto Group { get; set; }

        public Guid? TeacherId { get; set; }
        [JsonIgnore]
        public UserDto Teacher { get; set; }

        public Guid? SubjectId { get; set; }
        [JsonIgnore]
        public SubjectDto Subject { get; set; }

        public string RoomName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [JsonIgnore]
        public List<AttendanceDto> Attendances { get; set; } = new List<AttendanceDto>();
    }

}
