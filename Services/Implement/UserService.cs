 using RedmineApp.Models.CommonModels;
using RedmineApp.Models.DtoModels.Position;
using RedmineApp.Models.DtoModels.User;
using RedmineApp.Models.EntityModels;
using RedmineApp.Repositories.Interfaces;
using RedmineApp.Services.Interfaces;

namespace RedmineApp.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommonService _commonService;
        private readonly IPositionRepository _positionRepository;
        private readonly IEmailService _emailService;
        public UserService(IUserRepository userRepository, ICommonService commonService, IPositionRepository positionRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _commonService = commonService;
            _positionRepository = positionRepository;
            _emailService = emailService;

        }
        private string GenerateRandomPassword(int length = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<ApiResponse> DeleteUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if(user == null)
            {
                return ApiResponse.Response(404, "Không tìm thấy User");

            }
            string position = _commonService.GetPosition();
            if (position != "Admin")
            {
                return ApiResponse.Response(403, "Bạn không có quyền xóa User");
            }
            var isDeleted = await _userRepository.DeleteByIdAsync(id);
            if(!isDeleted)
            {
                return ApiResponse.Response(500, "Xóa User thất bại");
            }
            return ApiResponse.Response(200, "Xóa User thành công");
        }

        public async Task<ApiResponse> GetAllUserAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var result = users.Where(u => !u.DeleteFlg).Select(u => new UserResponseDto
            {
                Id = u.Id,
                Code = u.Code,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                JoinDate = u.JoinedDate ?? DateTime.UtcNow,
                Status = u.IsActive,

            }).ToList();
            return ApiResponse.Response(200, "Thành công", result);
        }

        public async Task<ApiResponse> GetMeAsync()
        {
            int userId = _commonService.GetUserId();
            var user = await _userRepository.GetUserDetailByIdAsync(userId);
            if (user == null || user.DeleteFlg)
            {
                return ApiResponse.Response(404, "Tài khoản không tồn tại hoặc đã bị khóa.");
            }
            var userDto = new UserDetailDto
            {
                Id = user.Id,
                Code = user.Code,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Birthday = user.Birthday,
                JoinedDate = user.JoinedDate,
                LeavedDate = user.LeavedDate,
                IsActive = user.IsActive,
                Positions = user.Positions.Select(p => new PositionDto
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList()
            };
            return ApiResponse.Response(200, "Thành công", userDto);
        }

        public async Task<ApiResponse> GetUserByIdAsync(int id)
        {
            var user  = await _userRepository.GetUserDetailByIdAsync(id);
            if (user == null)
            {
                return ApiResponse.Response(404, "Không tìm thấy người dùng");
            }
            var userDto = new UserDetailDto
            {
                Id = user.Id,
                Code = user.Code,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Birthday = user.Birthday,
                JoinedDate = user.JoinedDate,
                LeavedDate = user.LeavedDate,
                IsActive = user.IsActive,
                Positions = user.Positions.Select(p => new PositionDto
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList()
            };
            return ApiResponse.Response(200, "Thành công", userDto);
        }

        public async Task<ApiResponse> GetUserInfoById(int id)
        {
            var user = await _userRepository.GetUserDetailByIdAsync(id);
            if (user == null)
            {
                return ApiResponse.Response(404, "Khong tim thay User");
            }
            return ApiResponse.Response(201, "Da tim thay User!", user);
        }

        public async Task<ApiResponse> UpdateUserAsync(int id, UpdateUserRequestDto dto)
        {
            var user = await _userRepository.GetUserDetailByIdAsync(id);
            if(user == null)
            {
                return ApiResponse.Response(404, "Không tìm thấy User");
            }
            if(_commonService.GetUserId() != id && _commonService.GetPosition() != "Admin")
            {
                return ApiResponse.Response(403, "Bạn không có quyền cập nhật User này");
            }
            user.Code = dto.Code;
            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Gender = dto.Gender;
            user.PhoneNumber = dto.PhoneNumber;
            user.Birthday = dto.Birthday;
            user.JoinedDate = dto.JoinedDate;
            user.LeavedDate = dto.LeavedDate;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.Now;
            user.Positions.Clear();
            var positionsId = dto.Positions.Select(p => p.Id).ToList();
            var existingPositions = await _userRepository.GetPositionsByIdsAsync(positionsId);
            foreach (var pos in existingPositions)
            {
                if(!user.Positions.Contains(pos))
                {
                    user.Positions.Add(pos);
                }
            }
            await _userRepository.SaveChangesAsync();
            return ApiResponse.Response(200, "Cập nhật User thành công");
        }

        public async Task<ApiResponse> CreateUserAsync(CreateUserRequestDto dto)
        {
            bool isExisitingEmail = await _userRepository.ExistsByEmailAsync(dto.Email) ; 
            bool isExistingCode = await _userRepository.ExistsByCodeAsync(dto.Code) ;
            if(isExistingCode || isExisitingEmail)
            {
                return ApiResponse.Response(400, "Mã nhân viên hoặc Email đã tồn tại trong hệ thống.");
            }
            string rawPassword = GenerateRandomPassword();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);
            var newUser = new User
            {
                Code = dto.Code,
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                IsActive = dto.IsActive,
                Password = hashedPassword
            };
            if(dto.PositionIds != null && dto.PositionIds.Any())
            {
                newUser.Positions = await _positionRepository.GetPositionsByIdsAsync(dto.PositionIds);
            }
            await _userRepository.CreateAsync(newUser);
            await _userRepository.SaveChangesAsync();
            var responseData = new CreateUserResponseDto
            {
                UserId = newUser.Id,
                Name= dto.Name,
                Email = newUser.Email,
                GeneratedPassword = rawPassword
            };
            try
            {
                Console.WriteLine($"Đang chuẩn bị gửi email cho: {newUser.Email}");
                await _emailService.SendRandomPassword(newUser.Email, rawPassword);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Gửi mail thất bại: {ex.Message}");
            }
            return ApiResponse.Response(200, "Tạo User khoản thành công!", responseData);
        }

        public async Task<ApiResponse> CheckEmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ApiResponse.Response(400, "Email không được để trống");
            }
            bool isExist = await _userRepository.ExistsByEmailAsync(email.Trim());
            if(!isExist)
            {
                return ApiResponse.Response(404, "Email không tồn tại trong hệ thống. Vui lòng kiểm tra lại!");
            }
            return ApiResponse.Response(200, "Email hợp lệ. Đang tiến hành gửi mã xác nhận...");
        }
    }
}
