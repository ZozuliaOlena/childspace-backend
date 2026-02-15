using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Schedule Lesson { get; set; }

        public int ChildId { get; set; }
        [ForeignKey("ChildId")]
        public virtual Child Child { get; set; }

        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
    }
}
