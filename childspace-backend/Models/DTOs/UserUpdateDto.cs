namespace childspace_backend.Models.DTOs
{
    public class UserUpdateDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? CenterId { get; set; }
    }
}
