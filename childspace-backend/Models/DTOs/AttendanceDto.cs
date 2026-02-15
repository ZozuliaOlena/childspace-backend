using childspace_backend.Models.Enums;

namespace childspace_backend.Models.DTOs
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }

        public Guid LessonId { get; set; }
        public ScheduleDto Lesson { get; set; } 

        public Guid ChildId { get; set; }
        public ChildDto Child { get; set; } 

        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
    }

}
