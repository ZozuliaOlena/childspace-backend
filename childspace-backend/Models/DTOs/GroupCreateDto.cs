namespace childspace_backend.Models.DTOs
{
    public class GroupCreateDto
    {
        public string Name { get; set; }

        public Guid CenterId { get; set; }

        public Guid? TeacherId { get; set; }

        public string? Description { get; set; }
    }
}
