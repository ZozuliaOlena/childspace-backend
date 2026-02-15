namespace childspace_backend.Models.DTOs
{
    public class UserCreateRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid? CenterId { get; set; }

        public string Password { get; set; }
    }
}
