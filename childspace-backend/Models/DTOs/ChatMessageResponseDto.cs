namespace childspace_backend.Models.DTOs
{
    public class ChatMessageResponseDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
    }
}
