
using RedmineApp.Models.DtoModels.User;
using RedmineApp.Models.EntityModels;

namespace RedmineApp.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByCodeAsync(string code);
        Task<User?> GetLastUserAsync();
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserDetailByIdAsync(int id);
        Task<List<Position>> GetPositionsByIdsAsync(List<int> ids);
        Task<List<User>> GetUsersNotInProjectAsync(int projectId);
        Task UpdateUserAsync(User user);
    }
}
