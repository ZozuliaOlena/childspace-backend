namespace childspace_backend.Models.DTOs
{
    public class GroupChildDto
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }
        public GroupDto Group { get; set; }

        public Guid ChildId { get; set; }
        public ChildDto Child { get; set; }
    }
}
