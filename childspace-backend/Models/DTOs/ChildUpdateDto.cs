namespace childspace_backend.Models.DTOs
{
    public class UpdateChildDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Notes { get; set; }
    }
}
