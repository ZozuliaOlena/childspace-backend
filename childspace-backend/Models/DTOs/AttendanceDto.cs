using childspace_backend.Models.Enums;
using System.Text.Json.Serialization;

namespace childspace_backend.Models.DTOs
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }

        public Guid LessonId { get; set; }

        [JsonIgnore]
        public ScheduleDto Lesson { get; set; } 

        public Guid ChildId { get; set; }

        [JsonIgnore]
        public ChildDto Child { get; set; } 

        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
    }

}
