namespace childspace_backend.Models.DTOs
{
    public class MessageDto
    {
        public Guid Id { get; set; }

        public Guid UserChatId { get; set; }
        public UserChatDto UserChat { get; set; }

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
