namespace childspace_backend.Models.DTOs
{
    public class SendMessageDto
    {
        public Guid ChatId { get; set; }
        public string Content { get; set; }
    }
}
