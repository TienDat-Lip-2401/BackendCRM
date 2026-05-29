using RedmineApp.Models.CommonModels;
using RedmineApp.Models.DtoModels.Position;
using RedmineApp.Models.EntityModels;
using RedmineApp.Repositories.Interfaces;

namespace RedmineApp.Services.Interfaces
{
    public interface IPositionService
    {
        Task<ApiResponse> GetPositionsByUserIdAsync(int userId);
        Task<ApiResponse> AddPositionListToUserAsync(AssignPositionListRequestDto dto);
        Task<ApiResponse> RemovePositionFromUserAsync(DeletePositionRequest dto);
        Task<ApiResponse> GetAllPositonsAsync();
        Task<ApiResponse> GetAvailablePositionsAsync(int userId);


    }
}
