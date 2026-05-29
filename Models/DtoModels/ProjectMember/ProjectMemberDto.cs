namespace RedmineApp.Models.DtoModels.ProjectMember
{
    public class ProjectMemberDto
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        // Thông tin Vai trò (Role)
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
