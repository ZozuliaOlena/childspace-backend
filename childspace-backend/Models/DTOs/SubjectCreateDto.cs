namespace childspace_backend.Models.DTOs
{
    public class SubjectCreateDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public Guid CenterId { get; set; }
    }
}
