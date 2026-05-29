using RedmineApp.Models.DtoModels.ProjectMember;

namespace RedmineApp.Models.DtoModels.Project
{
    public class CreateProjectRequestDto
    {
        public string ProjectCode { get; set; } = null!;
        public string Title { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public int? ProjectManagerId { get; set; }
        public int? Status { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsActive { get; set; }
        public List<ProjectMemberRequestDto> MemberIds { get; set; } = new List<ProjectMemberRequestDto>();
    }
}
