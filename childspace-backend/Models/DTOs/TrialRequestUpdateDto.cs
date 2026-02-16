namespace childspace_backend.Models.DTOs
{
    public class TrialRequestUpdateDto
    {
        public string ParentName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string ChildName { get; set; }

        public Guid ChildAge { get; set; }

        public string? Comment { get; set; }
    }
}
