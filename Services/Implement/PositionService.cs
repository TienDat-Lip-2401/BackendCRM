using RedmineApp.Models.CommonModels;
using RedmineApp.Models.DtoModels.Position;
using RedmineApp.Models.EntityModels;
using RedmineApp.Repositories.Interfaces;
using RedmineApp.Services.Interfaces;

namespace RedmineApp.Services.Implement
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IUserRepository _userRepository;
        public PositionService(IPositionRepository positionRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _positionRepository = positionRepository;
        }
        
        public async Task<ApiResponse> AddPositionListToUserAsync(AssignPositionListRequestDto dto)
        {
            var userId = dto.UserId;
            var user = await _positionRepository.GetUserWithPositionsAsync(userId);
            if (user == null) return ApiResponse.Response(404, "Không tìm thấy User");
            var positions = await _positionRepository.GetPositionsByIdsAsync(dto.PositionIds);
            int count = 0;
            foreach(var pos in positions)
            {
                if(!user.Positions.Any(p => p.Id == pos.Id))
                {
                    count++;
                    user.Positions.Add(pos);
                }
            }
            if(count > 0)
            {
                await _positionRepository.SaveChangesAsync();
                return ApiResponse.Response(200, $"Đã thêm thành công {count} chức vụ cho User");
            }
            return ApiResponse.Response(400, "User đã đảm nhiệm tất cả các chức vụ này rồi");
        
        }

        public async Task<ApiResponse> GetAllPositonsAsync()
        {
            var positions = await _positionRepository.GetAllAsync();
            var result = positions.Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
            });
            return ApiResponse.Response(200, "Thành công", result);
        }

        public async Task<ApiResponse> GetAvailablePositionsAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if(user == null)
            {
                return ApiResponse.Response(404, "Không tìm thấy User");
            }
            var positions = await _positionRepository.GetAvailablePositionsForUserAsync(userId);
            var result = positions.Select(p => new PositionDto {
                Id = p.Id,
                Name= p.Name,
            });

            return ApiResponse.Response(200, "Thành công", result);
        }

        public async Task<ApiResponse> GetPositionsByUserIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return ApiResponse.Response(404, "Không tìm thấy User");
            var positions = await _positionRepository.GetPositionsByUserIdAsync(userId);
            var result = positions.Select(x => new PositionDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
            return ApiResponse.Response(200, "Thành công", result);
        }

        public async Task<ApiResponse> RemovePositionFromUserAsync(DeletePositionRequest dto)
        {
            var user = await _userRepository.GetUserByIdAsync(dto.userId);
            if (user == null) return ApiResponse.Response(404, "Không tìm thấy User ở PositonService");
            var positionRemove = user.Positions.FirstOrDefault(p => p.Id == dto.positionId);
            if (positionRemove == null)
            {
                return ApiResponse.Response(404, "Người dùng không đảm nhiệm chức vụ này");
            }
            user.Positions.Remove(positionRemove);
            await _userRepository.SaveChangesAsync();
            return ApiResponse.Response(200, "Đã xóa chức vụ khỏi người dùng thành công");
        }

    }
}
