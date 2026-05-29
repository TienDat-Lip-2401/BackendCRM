using Microsoft.AspNetCore.Authorization;
using RedmineApp.Models.CommonModels;
using RedmineApp.Repositories.Interfaces;
using RedmineApp.Services.Interfaces;

namespace RedmineApp.MiddleWares
{
    public class CustomAuthorizeMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomAuthorizeMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(
            HttpContext context,
            ITokenService tokenService,
            ICommonService commonService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            var pathLower = path.ToLower();
            if (!string.IsNullOrEmpty(path) &&
               (pathLower.StartsWith("/swagger") ||
                pathLower.Contains("/api/auth/login") ||
                pathLower.Contains("/api/auth/register") ||
                pathLower.Contains("/api/auth/refresh-token")))
            {
                await _next(context); // Cho đi thẳng vào Controller!
                return;
            }
            // Bypass middleware nếu endpoint có attribute [AllowAnonymous]
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
            {
                await _next(context);
                return;
            }// Lấy token từ header Authorization
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                token = context.Request.Cookies["access_token"];
            }
            if (string.IsNullOrEmpty(token))
            {
                await WriteUnauthorized(context, $"[Code mới] Bị chặn Token tại link: '{pathLower}'");
                return;
            }
            // Kiểm tra tính hợp lệ của token
            var isTokenValid = tokenService.ValidateAccessToken(token);
            if (!isTokenValid)
            {
                await WriteUnauthorized(context, "Token không hợp lệ hoặc đã hết hạn123.");
                return;
            }
            // Phân tích token để lấy thông tin claim
            var jwtSecurityToken = tokenService.ParseToken(token);
            // Lấy thông tin refresh token id từ claim để kiểm tra phiên làm việc
            var refreshToken = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "refresh_token_id");
            if (refreshToken == null || !int.TryParse(refreshToken.Value, out int refreshTokenId))
            {
                await WriteUnauthorized(context, "Token không chứa định danh phiên làm việc.");
                return;
            }
            // Kiểm tra refresh token trong database để xác thực phiên làm việc
            var rfToken = await refreshTokenRepository.GetByIdAsync(refreshTokenId);
            if (rfToken == null || rfToken.IsRevoked)
            {
                await WriteUnauthorized(context, "Phiên đăng nhập đã hết hạn hoặc bạn đã đăng xuất.");
                return;
            }
            // Lấy thông tin user id từ claim để xác thực người dùng
            var userIdClaim = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "nameid" || x.Type == "user_id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                await WriteUnauthorized(context, "Dữ liệu Token bị sai lệch.");
                return;
            }
            // Kiểm tra thông tin người dùng trong database
            var user = await userRepository.GetUserByIdAsync(userId);

            if (user == null || user.DeleteFlg)
            {
                await WriteUnauthorized(context, "Tài khoản đã bị vô hiệu hóa.");
                return;
            }
            var positionNames = user.Positions?.Select(p => p.Name).ToList() ?? new List<string>();
            string selectedPosition = "Staff";
            if (positionNames.Any()) // Kiểm tra xem danh sách có phần tử nào không
            {
                if (positionNames.Contains("Admin", StringComparer.OrdinalIgnoreCase))
                {
                    selectedPosition = "Admin";
                }
                else if (positionNames.Contains("Project Manager", StringComparer.OrdinalIgnoreCase) ||
                         positionNames.Contains("PM", StringComparer.OrdinalIgnoreCase))
                {
                    selectedPosition = "Project Manager";
                }
                else
                {
                    // Dùng FirstOrDefault() thay vì First() để an toàn tuyệt đối
                    selectedPosition = positionNames.FirstOrDefault() ?? "Staff";
                }
            }
            Console.WriteLine(selectedPosition + "Permission");
            // Lưu thông tin user id vào CommonService để sử dụng trong các service khác
            commonService.SetUserId(userId);
            commonService.SetPosition(selectedPosition);
            await _next(context);
        }
        private async Task WriteUnauthorized(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(ApiResponse.Response(401, message));
        }
    }
}
