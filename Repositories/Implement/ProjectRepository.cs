using Microsoft.EntityFrameworkCore;
using RedmineApp.Models.EntityModels;
using RedmineApp.Repositories.Interfaces;

namespace RedmineApp.Repositories.Implement
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void AddMember(ProjectMember member)
        {
            _context.ProjectMembers.Add(member);
        }

        public async Task<Project?> GetProjectDetailAsync(int projectId)
        {
            return await _context.Projects
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.Role)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .FirstOrDefaultAsync(p => p.Id == projectId && !p.DeleteFlg);
        }

        public async Task<List<Project>> GetProjectsByUserIdAsync(int userId)
        {
            return await _context.Projects
                .Where(p => !p.DeleteFlg)
                .Where(p => p.ProjectMembers.Any(pm => pm.UserId == userId))
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.Role)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public void RemoveMembers(IEnumerable<ProjectMember> members)
        {
            _context.ProjectMembers.RemoveRange(members);
        }
    }
}
