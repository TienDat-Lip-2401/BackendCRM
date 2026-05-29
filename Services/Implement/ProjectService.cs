using RedmineApp.Models.CommonModels;
using RedmineApp.Models.DtoModels.Project;
using RedmineApp.Models.DtoModels.ProjectMember;
using RedmineApp.Models.DtoModels.User;
using RedmineApp.Models.EntityModels;
using RedmineApp.Repositories.Implement;
using RedmineApp.Repositories.Interfaces;
using RedmineApp.Services.Interfaces;
namespace RedmineApp.Services.Implement

{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICommonService _commonService;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IUserRepository _userRepository;
        public ProjectService(IProjectRepository projectRepository, ICommonService commonService, IProjectMemberRepository projectMemberRepository, IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _commonService = commonService;
            _projectMemberRepository = projectMemberRepository;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse> AddMembersToProjectAsync(int projectId, List<ProjectMemberRequestDto> memberDtos)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.DeleteFlg)
            {
                return ApiResponse.Response(404, "Không tìm thấy dự án!");
            }
            var newMembers = new List<ProjectMember>();
            foreach (var dto in memberDtos)
            {
                bool isExist = await _projectMemberRepository.IsUserInProjectAsync(projectId, dto.UserId);
                if(!isExist)
                {
                    newMembers.Add(new ProjectMember {
                        ProjectId = projectId, 
                        UserId = dto.UserId,
                        RoleId = dto.RoleId,
                    });

                }
            }
            if (newMembers.Any())
            {
                await _projectMemberRepository.CreateRangeAsync(newMembers);
                await _projectMemberRepository.SaveChangesAsync();
                return ApiResponse.Response(200, $"Đã thêm thành công {newMembers.Count} thành viên vào dự án.");
            }
            return ApiResponse.Response(400, "Các thành viên này đã tồn tại trong dự án.");
        }

        public async Task<ApiResponse> CreateProjectAsync(int currentUserId, CreateProjectRequestDto dto)
        {
            try
            {
                if (dto.StartDate > dto.EndDate)
                {
                    return ApiResponse.Response(400, "Ngày bắt đầu không được lớn hơn ngày kết thúc!");
                }
                var newProject = new Project
                {
                    ProjectCode = dto.ProjectCode,
                    Title = dto.Title,
                    Description = dto.Description,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    ManagerId = dto.ProjectManagerId ?? currentUserId,
                    Status = dto.Status ?? 1,
                    IsPublic = dto.IsPublic ?? false,
                    IsActive = dto.IsActive ?? true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    DeleteFlg = false,
                    ProjectMembers = new List<ProjectMember>()
                };
                if (dto.MemberIds != null && dto.MemberIds.Any())
                {
                    var uniqueMembers = dto.MemberIds
                        .GroupBy(m => m.UserId)
                        .Select(g => g.First())
                        .ToList();
                    foreach (var memberDto in uniqueMembers)
                    {
                        newProject.ProjectMembers.Add(
                                new ProjectMember
                                {
                                    UserId = memberDto.UserId,
                                    RoleId = memberDto.RoleId,
                                }
                            );
                    }
                }
                await _projectRepository.CreateAsync(newProject);
                bool isSaved = await _projectRepository.SaveChangesAsync();
                if (!isSaved)
                {
                    return ApiResponse.Response(500, "Lỗi hệ thống khi lưu dự án vào cơ sở dữ liệu.");
                }

                // 5. Trả về kết quả
                return ApiResponse.Response(201, "Tạo dự án và phân quyền thành viên thành công!", new
                {
                    ProjectId = newProject.Id,
                    ProjectCode = newProject.ProjectCode
                });
            }
            catch (Exception ex)
            {
                // Bắt lỗi exception (VD: Lỗi kết nối DB)
                return ApiResponse.Response(500, $"Lỗi server: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteProjectByIdAsync(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if(project == null)
            {
                return ApiResponse.Response(404, "Không tìm thấy Project");
            }
            var isDeleted = await _projectRepository.DeleteByIdAsync(projectId);
            if(isDeleted == false)
            {
                return ApiResponse.Response(500, "Xóa Project thất bại");
            }
            return ApiResponse.Response(200, "Xóa Project thành công");
        }

        public async Task<ApiResponse> GetAvailableUsersForProjectAsync(int projectId)
        {
            try
            {
                // Gọi Repo lấy dữ liệu
                var users = await _userRepository.GetUsersNotInProjectAsync(projectId);

                // Map sang DTO
                var userDtos = users.Select(u => new AvailableUserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                }).ToList();

                return ApiResponse.Response(200, "Lấy danh sách user thành công", userDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse.Response(500, $"Lỗi server: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetProjectDetailAsync(int projectId)
        {
            var project = await _projectRepository.GetProjectDetailAsync(projectId);

            if (project == null || project.DeleteFlg)
            {
                return ApiResponse.Response(404, "Không tìm thấy dự án hoặc dự án đã bị xóa!");
            }
            var projectDto = new ProjectResponseDto
            {
                Id = project.Id,
                Title = project.Title,
                ProjectCode = project.ProjectCode,
                Description = project.Description,
                ProjectManagerId= project.ManagerId,
                isActive = project.IsActive,
                CreatedAt = project.CreatedAt,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                isPublic = project.IsPublic,
                Members = project.ProjectMembers.Select(pm => new ProjectMemberDto
                {
                    ProjectId= pm.ProjectId,
                    UserId = pm.UserId,
                    UserName= pm.User?.Name ?? "Unknown",
                    UserEmail = pm.User?.Email ?? "Unknown",
                    RoleId = pm.RoleId,
                    RoleName= pm.Role?.Name ?? "Unknown"
                }).ToList(),
            };
            return ApiResponse.Response(200, "Lấy chi tiết dự án thành công", projectDto);
        }

        public async Task<ApiResponse> GetProjectsByUserIdAsync(int userId)
        {
            var projectEntities = await _projectRepository.GetProjectsByUserIdAsync(userId);
            var projectDtos = projectEntities.Select(projectEntity => new ProjectResponseDto {
                Id = projectEntity.Id,
                Title = projectEntity.Title,
                ProjectCode = projectEntity.ProjectCode,
                CreatedAt = projectEntity.CreatedAt,
                StartDate = projectEntity.StartDate,
                EndDate= projectEntity.EndDate,
                Description = projectEntity.Description,
                ProjectManagerId = projectEntity.ManagerId,
                isActive = projectEntity.IsActive,
                Members = projectEntity.ProjectMembers.Select(pm => new ProjectMemberDto
                {
                    ProjectId = pm.ProjectId,
                    UserId = pm.UserId,
                    UserName = pm.User?.Name ?? "Unknown",
                    UserEmail = pm.User?.Email ?? "Unknown",
                    RoleId = pm.RoleId,
                    RoleName = pm.Role?.Name ?? "Unknown"
                }).ToList()
            }).ToList();

            return ApiResponse.Response(200, "Lấy danh sách dự án của người dùng thành công", projectDtos);
        }

        public async Task<ApiResponse> GetProjectsForCurrentUserAsync()
        {
            int userId = _commonService.GetUserId();
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse.Response(404, "Không tìm thấy thông tin tài khoản!");
            }
            var currentPositionUser = _commonService.GetPosition();
            List<Project> projects;
            if (currentPositionUser == "Admin")
            {
                projects = await _projectRepository.GetAllAsync();

            }
            else
            {
                projects = await _projectRepository.GetProjectsByUserIdAsync(userId);
            }
            var projectDtos = projects.Select(projectEntity => new ProjectResponseDto
            {
                Id = projectEntity.Id,
                Title = projectEntity.Title,
                ProjectCode = projectEntity.ProjectCode,
                CreatedAt = projectEntity.CreatedAt,
                StartDate = projectEntity.StartDate,
                EndDate = projectEntity.EndDate,
                Description = projectEntity.Description,
                ProjectManagerId = projectEntity.ManagerId,
                isActive = projectEntity.IsActive,
                Status = projectEntity.Status,
                Members = projectEntity.ProjectMembers.Select(pm => new ProjectMemberDto
                {
                    ProjectId = pm.ProjectId,
                    UserId = pm.UserId,
                    UserName = pm.User?.Name ?? "Unknown",
                    UserEmail = pm.User?.Email ?? "Unknown",
                    RoleId = pm.RoleId,
                    RoleName = pm.Role?.Name ?? "Unknown"
                }).ToList()
            }).ToList();
            return ApiResponse.Response(200, "Lấy danh sách dự án thành công!", projectDtos);
        }

        public async Task<ApiResponse> RemoveMemberFromProjectAsync(int projectId, int userId)
        {
            try
            {
                var member = await _projectMemberRepository.GetByProjectAndUserAsync(projectId, userId);
                if (member == null)
                {
                    return ApiResponse.Response(404, "Thành viên này không tồn tại trong dự án!");
                }
                _projectMemberRepository.Delete(member);
                bool isSaved = await _projectMemberRepository.SaveChangesAsync();
                if (!isSaved)
                {
                    return ApiResponse.Response(500, "Lỗi hệ thống khi xóa thành viên khỏi dự án.");
                }
                return ApiResponse.Response(200, "Đã xóa cứng thành viên khỏi dự án thành công!");
            }
            catch (Exception ex)
            {
                return ApiResponse.Response(500, $"Lỗi server: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateProjectAsync(int projectId, UpdateProjectRequestDto dto)
        {
            var project = await _projectRepository.GetProjectDetailAsync(projectId);
            if(project == null)
            {
                return new ApiResponse { StatusCode = 404, Message = "Không tìm thấy dự án!" };
            }
            project.Title = dto.Title;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.Description = dto.Description;
            project.Status = dto.Status;
            project.IsPublic = dto.IsPublic;
            project.IsActive = dto.IsActive;
            var incomingMemberIds = dto.MemberIds ?? new List<UpdateMemberDto>();
            var incomingUserIds = incomingMemberIds.Select(m => m.UserId).ToList();
            var membersToRemove = project.ProjectMembers
            .Where(m => !incomingUserIds.Contains(m.UserId))
            .ToList();
            if(membersToRemove.Any() )
            {
                foreach( var memberToRemove in membersToRemove )
                {
                    _projectMemberRepository.Delete(memberToRemove);
                }
            }
            foreach (var incomingMember in incomingMemberIds)
            {
                var existingMember = project.ProjectMembers
                    .FirstOrDefault(m => m.UserId == incomingMember.UserId);
                if(existingMember == null)
                {
                    _projectRepository.AddMember(new ProjectMember {
                        ProjectId =projectId,
                        UserId = incomingMember.UserId,
                        RoleId = incomingMember.RoleId,
                    });

                }
                else
                {
                    if (existingMember.RoleId != incomingMember.RoleId)
                    {
                        existingMember.RoleId = incomingMember.RoleId;
                    }
                }
            }
            await _projectRepository.SaveChangesAsync();
            return new ApiResponse { StatusCode = 200, Message = "Cập nhật thành công!" };
        }
    }
}
