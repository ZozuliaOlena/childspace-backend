namespace childspace_backend.Models.DTOs
{
    public class ChildDto
    {
        public Guid Id { get; set; }

        public Guid CenterId { get; set; }
        public CenterDto Center { get; set; }

        public Guid ParentId { get; set; }
        public UserDto Parent { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Notes { get; set; }

        public List<GroupChildDto> Groups { get; set; } = new List<GroupChildDto>();
        public List<AttendanceDto> Attendances { get; set; } = new List<AttendanceDto>();
    }

}
