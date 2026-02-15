using System.Text.Json.Serialization;

namespace childspace_backend.Models.DTOs
{
    public class UserChatDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        [JsonIgnore]
        public UserDto User { get; set; }

        public Guid ChatId { get; set; }
        [JsonIgnore]
        public ChatDto Chat { get; set; }

        [JsonIgnore]
        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
    }
}
