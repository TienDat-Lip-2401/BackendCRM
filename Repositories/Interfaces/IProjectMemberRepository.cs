using RedmineApp.Models.EntityModels;

namespace RedmineApp.Repositories.Interfaces
{
    public interface IProjectMemberRepository
    {
        Task CreateRangeAsync(List<ProjectMember> projectMembers);

        // Lấy thông tin thành viên trong 1 dự án cụ thể (dùng để xóa)
        Task<ProjectMember?> GetByProjectAndUserAsync(int projectId, int userId);

        // Kiểm tra xem user đã tồn tại trong dự án chưa (dùng để tránh thêm trùng)
        Task<bool> IsUserInProjectAsync(int projectId, int userId);
        Task<bool> SaveChangesAsync();
        void Delete(ProjectMember member);
    }
}
