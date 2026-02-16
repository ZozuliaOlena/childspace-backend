namespace childspace_backend.Models.DTOs
{
    public class GroupUpdateDto
    {
        public string Name { get; set; }

        public Guid? TeacherId { get; set; }

        public string? Description { get; set; }
    }
}
