namespace childspace_backend.Models.DTOs
{
    public class SubjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public Guid CenterId { get; set; }
        public CenterDto Center { get; set; }
    }
}
