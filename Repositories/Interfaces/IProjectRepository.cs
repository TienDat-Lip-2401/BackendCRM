using RedmineApp.Models.EntityModels;

namespace RedmineApp.Repositories.Interfaces
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        Task<Project?> GetProjectDetailAsync(int projectId);
        Task<List<Project>> GetProjectsByUserIdAsync(int userId);
        void RemoveMembers(IEnumerable<ProjectMember> members);
        void AddMember(ProjectMember member);
    }
}
