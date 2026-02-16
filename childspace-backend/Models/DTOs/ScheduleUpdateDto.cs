namespace childspace_backend.Models.DTOs
{
    public class ScheduleUpdateDto
    {
        public Guid? TeacherId { get; set; }

        public Guid? SubjectId { get; set; }

        public string RoomName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
