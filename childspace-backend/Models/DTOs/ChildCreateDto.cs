namespace childspace_backend.Models.DTOs
{
    public class CreateChildDto
    {
        public Guid CenterId { get; set; }
        public Guid ParentId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Notes { get; set; }
    }
}
