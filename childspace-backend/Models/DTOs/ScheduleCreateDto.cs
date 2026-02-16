namespace childspace_backend.Models.DTOs
{
    public class ScheduleCreateDto
    {
        public Guid GroupId { get; set; }

        public Guid? TeacherId { get; set; }

        public Guid? SubjectId { get; set; }

        public string RoomName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
