using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RedmineApp.Filter;
using RedmineApp.Models.CommonModels;
using RedmineApp.Models.DtoModels.User;
using RedmineApp.Services.Interfaces;

namespace RedmineApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICommonService _commonService;
        public UserController(IUserService userService, ICommonService commonService)
        {
            _userService = userService;
            _commonService = commonService;
        }
        [HttpGet]
        [Route("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUserAsync();
            return Ok(users);
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _userService.DeleteUserByIdAsync(id);
            if (response.StatusCode == 404)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequestDto dto)
        {
            int currentUserId = _commonService.GetUserId();
            string currentUserPosition = _commonService.GetPosition();
            if (currentUserPosition != "Admin" && currentUserId != id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    StatusCode = 403,
                    Message = "Bạn không có quyền sửa thông tin của người dùng này!",
                });
            }
            var response = await _userService.UpdateUserAsync(id, dto);
            if (response.StatusCode == 404) return NotFound(response);
            if (response.StatusCode == 400) return BadRequest(response);
            if (response.StatusCode == 500) return StatusCode(500, response);

            return Ok(response);
        }
        [AuthorizeRole(["Admin"])]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto dto)
        {
            var response = await _userService.CreateUserAsync(dto);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet]
        [Route("check-email")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
        {
            var result = await _userService.CheckEmailExistsAsync(email);

            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {

            var result = await _userService.GetMeAsync();
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("{id}")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> GetUserDetailById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (result.StatusCode == 400)
            {
                return BadRequest(result);
            }
            if (result.StatusCode == 404)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
