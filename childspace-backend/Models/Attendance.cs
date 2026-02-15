using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using childspace_backend.Models.Enums;

namespace childspace_backend.Models
{
    public class Attendance
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Schedule Lesson { get; set; }

        public Guid ChildId { get; set; }
        [ForeignKey("ChildId")]
        public virtual Child Child { get; set; }

        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
    }
}
