using System.Text.Json.Serialization;

namespace childspace_backend.Models.DTOs
{
    public class GroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid CenterId { get; set; }

        public Guid? TeacherId { get; set; }

        public string? Description { get; set; }

        [JsonIgnore]
        public List<GroupChildDto>? GroupChildren { get; set; }
        [JsonIgnore]
        public List<ScheduleDto>? Schedules { get; set; }
        [JsonIgnore]
        public List<MaterialDto>? Materials { get; set; }
    }
}
