namespace childspace_backend.Models.DTOs
{
    public class UserProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public List<ChildProfileDto> Children { get; set; } = new List<ChildProfileDto>();
    }
}
