using RedmineApp.Models.CommonModels;
using RedmineApp.Models.DtoModels.Project;
using RedmineApp.Models.DtoModels.ProjectMember;

namespace RedmineApp.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ApiResponse> GetProjectDetailAsync(int projectId);
        Task<ApiResponse> GetProjectsByUserIdAsync(int userId);
        Task<ApiResponse> CreateProjectAsync(int currentUserId, CreateProjectRequestDto dto);
        Task<ApiResponse> AddMembersToProjectAsync(int projectId, List<ProjectMemberRequestDto> memberDtos);

        // Xóa 1 thành viên khỏi dự án
        Task<ApiResponse> RemoveMemberFromProjectAsync(int projectId, int userId);
        Task<ApiResponse> GetAvailableUsersForProjectAsync(int projectId);
        Task<ApiResponse> GetProjectsForCurrentUserAsync();
        Task<ApiResponse> UpdateProjectAsync(int projectId, UpdateProjectRequestDto dto);
        Task<ApiResponse> DeleteProjectByIdAsync(int projectId);
    }
}
