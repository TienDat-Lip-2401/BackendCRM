using RedmineApp.Models.EntityModels;

namespace RedmineApp.Repositories.Interfaces
{
    public interface IPositionRepository : IBaseRepository<Position>
    {
        Task<List<Position>> GetPositionsByUserIdAsync(int userId);

        Task<User?> GetUserWithPositionsAsync(int userId);
        
        Task<List<Position>> GetAvailablePositionsForUserAsync(int userId);
        Task<List<Position>> GetPositionsByIdsAsync(List<int> ids);

    }
}
