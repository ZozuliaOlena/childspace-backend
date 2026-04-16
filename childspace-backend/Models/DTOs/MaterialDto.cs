using childspace_backend.Models.Enums;
using System.Text.Json.Serialization;

namespace childspace_backend.Models.DTOs
{
    public class MaterialDto
    {
        public Guid Id { get; set; }

        public Guid CenterId { get; set; }

        public Guid? GroupId { get; set; }
        public string? GroupName { get; set; }

        public Guid SubjectId { get; set; }
        [JsonIgnore]
        public SubjectDto Subject { get; set; }
        public string SubjectName { get; set; }

        public Guid TeacherId { get; set; }
        [JsonIgnore]
        public UserDto Teacher { get; set; }
        public string TeacherName { get; set; }

        public string Title { get; set; }
        public string FileUrl { get; set; }
        public string? Description { get; set; }
        public MaterialType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
