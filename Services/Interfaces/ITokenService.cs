using System.IdentityModel.Tokens.Jwt;

namespace RedmineApp.Services.Interfaces
{
    public interface ITokenService
    {
        string ComputeSha256Hash(string rawData);
        // Tạo thẻ ngắn hạn (15 phút) dùng để gọi API
        string GenerateAccessToken(int userId, string username, string refreshTokenId);

        // Tạo thẻ dài hạn (7 ngày) dùng để đổi thẻ mới
        // Trả về Tuple gồm: Chuỗi thô (để gửi cho khách), Chuỗi băm (để lưu DB), và Hạn sử dụng
        (string RawToken, string TokenHash, DateTime ExpiresAt) GenerateRefreshToken();
        bool ValidateAccessToken(string token);
        JwtSecurityToken ParseToken(string token);
    }
}
