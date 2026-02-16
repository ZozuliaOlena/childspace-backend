namespace childspace_backend.Models.DTOs
{
    public class UserChatCreateDto
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
    }
}
