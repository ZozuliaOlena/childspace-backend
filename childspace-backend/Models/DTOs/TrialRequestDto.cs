namespace childspace_backend.Models.DTOs
{
    public class TrialRequestDto
    {
        public Guid Id { get; set; }

        public Guid CenterId { get; set; }
        public CenterDto Center { get; set; }

        public string ParentName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ChildName { get; set; }
        public Guid ChildAge { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
