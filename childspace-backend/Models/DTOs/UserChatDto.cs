namespace childspace_backend.Models.DTOs
{
    public class UserChatDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public UserDto User { get; set; }

        public Guid ChatId { get; set; }
        public ChatDto Chat { get; set; }

        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
    }
}
