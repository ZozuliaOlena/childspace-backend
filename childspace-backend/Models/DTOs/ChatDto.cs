using System.Text.Json.Serialization;

namespace childspace_backend.Models.DTOs
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public List<UserChatDto> Users { get; set; } = new List<UserChatDto>();
    }
}
