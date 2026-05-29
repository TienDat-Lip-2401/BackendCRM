using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RedmineApp.Models.DtoModels.Auth;
using RedmineApp.Services.Interfaces;

namespace RedmineApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ICommonService _commonService;
        public AuthController(IAuthService authService, IUserService userService, ICommonService commonService)
        {
            _authService = authService;
            _userService = userService;
            _commonService = commonService;
        }
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            var response = await _authService.RegisterAsync(dto);
            if (response.StatusCode == 400)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                             ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            var response = await _authService.LoginAsync(dto, ipAddress, userAgent);
            if (response.StatusCode == 400)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await _authService.LogoutAsync();
            if (response.StatusCode != 200) return BadRequest(response);
            return Ok(response);
        }
        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetUserInfo()
        {
            int userId = _commonService.GetUserId();
            var response = await _userService.GetMeAsync();

            if (response.StatusCode == 404) return NotFound(response);

            return Ok(response);
        }
        [HttpPost]
        [Route("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
        {
            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                           ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            var response = await _authService.RefreshTokenAsync(dto, ipAddress, userAgent);
            if (response.StatusCode != 200) return BadRequest(response);
            return Ok(response);
        }
        [HttpPost]
        [Route("reset-my-password")]
        public async Task<IActionResult> ResetMyPassword(ResetPasswordDto dto)
        {
            int userId = _commonService.GetUserId();
            var result = await _authService.ResetPasswordAsync(userId, dto.NewPassword);
            if (result.StatusCode == 404) return NotFound(result);

            return Ok(result);
        }
    }
}
