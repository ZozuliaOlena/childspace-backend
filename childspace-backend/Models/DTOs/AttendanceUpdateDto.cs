using childspace_backend.Models.Enums;

namespace childspace_backend.Models.DTOs
{
    public class AttendanceUpdateDto
    {
        public AttendanceStatus Status { get; set; }

        public string? Note { get; set; }
    }
}
