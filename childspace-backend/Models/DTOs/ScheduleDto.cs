namespace childspace_backend.Models.DTOs
{
    public class ScheduleDto
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }
        public GroupDto Group { get; set; }

        public Guid? TeacherId { get; set; }
        public UserDto Teacher { get; set; }

        public Guid? SubjectId { get; set; }
        public SubjectDto Subject { get; set; }

        public string RoomName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<AttendanceDto> Attendances { get; set; } = new List<AttendanceDto>();
    }

}
