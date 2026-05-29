
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedmineApp.Filter;
using RedmineApp.Models.DtoModels.Position;
using RedmineApp.Services.Interfaces;

namespace RedmineApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionController : Controller
    {
        private readonly IPositionService _positionService;
        private readonly ICommonService _commonService;
        public PositionController(IPositionService positionService, ICommonService commonService)
        {
            _positionService = positionService;
            _commonService = commonService;
        }
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllPositionsAsync()
        {
            var res = await _positionService.GetAllPositonsAsync();
            return Ok(res);
        }
        [HttpGet]
        [Route("user/{userId}")]
        public async Task<IActionResult> GetUserPositions(int userId)
        {
            var res = await _positionService.GetPositionsByUserIdAsync(userId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPositions()
        {
            var positions = await _positionService.GetAllPositonsAsync();
            return Ok(positions);
        }
        [HttpGet]
        [Route("available/{userId}")]
        public async Task<IActionResult> GetAvailablePositions(int userId)
        {
            var result = await _positionService.GetAvailablePositionsAsync(userId);
            return Ok(result);
        }
        [HttpPost]
        [Route("assign-list")]
        public async Task<IActionResult> AddPositions(AssignPositionListRequestDto dto)
        {
            var result =await _positionService.AddPositionListToUserAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
        [AuthorizeRole("Admin")]
        [HttpDelete]
        [Route("remove-position")]
        public async Task<IActionResult> RemovePosition([FromBody] DeletePositionRequest dto)
        {
            var result = await _positionService.RemovePositionFromUserAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
