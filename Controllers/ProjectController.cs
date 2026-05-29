using Microsoft.AspNetCore.Mvc;
using RedmineApp.Filter;
using RedmineApp.Models.DtoModels.Project;
using RedmineApp.Models.DtoModels.ProjectMember;
using RedmineApp.Services.Interfaces;

namespace RedmineApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ICommonService _commonService;
        public ProjectController(IProjectService projectService, ICommonService commonService)
        {
            _projectService = projectService;
            _commonService = commonService;
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProjectDetail(int id)
        {
            var result = await _projectService.GetProjectDetailAsync(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet]
        [Route("my-projects")]
        public async Task<IActionResult> GetMyProjects()
        {
            
            int currentUserId = _commonService.GetUserId();
            if (currentUserId <= 0)
            {
                return Unauthorized(new { StatusCode = 401, Message = "Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn!" });
            }

            var result = await _projectService.GetProjectsForCurrentUserAsync();
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequestDto dto)
        {
            int currentUserId = _commonService.GetUserId();
            if (currentUserId <= 0)
            {
                return Unauthorized(new { StatusCode = 401, Message = "Phiên đăng nhập không hợp lệ hoặc đã hết hạn!" });
            }
            var result = await _projectService.CreateProjectAsync(currentUserId, dto);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost]
        [Route("{projectId}/member")]
        [AuthorizeRole("Admin", "Project Manager")]
        public async Task<IActionResult> AddMembersToProject(int projectId, [FromBody] List<ProjectMemberRequestDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest(new { StatusCode = 400, Message = "Danh sách thành viên không được để trống" });
            }
            var result = await _projectService.AddMembersToProjectAsync(projectId, dtos);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete]
        [Route("{projectId}/member/{userId}")]
        [AuthorizeRole("Admin", "ProjectManager")]
        public async Task<IActionResult> RemoveMemberFromProject(int projectId, int userId)
        {
            var result = await _projectService.RemoveMemberFromProjectAsync(projectId, userId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet]
        [Route("{projectId}/available-users")]
        public async Task<IActionResult> GetAvailableUsers(int projectId)
        {
            var result = await _projectService.GetAvailableUsersForProjectAsync(projectId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut]
        [Route("{projectId}")]
        [AuthorizeRole("Admin", "Project Manager")]
        public async Task<IActionResult> UpdateProject(int projectId, [FromBody] UpdateProjectRequestDto dto)
        {
            var currentUserId = _commonService.GetUserId();
            if (currentUserId <= 0)
            {
                return Unauthorized(new { StatusCode = 401, Message = "Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn!" });
            }
            if (dto == null)
            {
                return BadRequest(new { StatusCode = 400, Message = "Dữ liệu gửi lên không hợp lệ!" });
            }
            var result = await _projectService.UpdateProjectAsync(projectId, dto);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete]
        [Route("{projectId}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var response = await _projectService.DeleteProjectByIdAsync(projectId);
            if (response.StatusCode == 404)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
