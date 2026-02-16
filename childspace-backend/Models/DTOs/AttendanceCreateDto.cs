using childspace_backend.Models.Enums;

namespace childspace_backend.Models.DTOs
{
    public class AttendanceCreateDto
    {
        public Guid LessonId { get; set; }

        public Guid ChildId { get; set; }

        public AttendanceStatus Status { get; set; }

        public string? Note { get; set; }
    }
}
