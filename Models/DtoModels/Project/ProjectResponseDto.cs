using RedmineApp.Models.DtoModels.ProjectMember;

namespace RedmineApp.Models.DtoModels.Project
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string ProjectCode { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public int? ProjectManagerId { get; set; }
        public bool? isActive { get; set; }
        public bool? isPublic { get; set; }
        public int? Status { get; set; }

        public List<ProjectMemberDto> Members { get; set; } = new List<ProjectMemberDto>();
    }
}
