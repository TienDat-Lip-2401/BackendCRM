namespace RedmineApp.Models.DtoModels.Project
{
    public class UpdateProjectRequestDto
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }
        public List<UpdateMemberDto> MemberIds { get; set; }
    }
}
