namespace childspace_backend.Models.DTOs
{
    public class MessageCreateDto
    {
        public Guid UserChatId { get; set; }
        public string Content { get; set; }
    }
}
