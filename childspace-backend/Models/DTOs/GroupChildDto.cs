using System.Text.Json.Serialization;

namespace childspace_backend.Models.DTOs
{
    public class GroupChildDto
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }
        [JsonIgnore]
        public GroupDto Group { get; set; }

        public Guid ChildId { get; set; }
        [JsonIgnore]
        public ChildDto Child { get; set; }
    }
}
