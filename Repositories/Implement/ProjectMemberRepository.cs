using Microsoft.EntityFrameworkCore;
using RedmineApp.Models.EntityModels;
using RedmineApp.Repositories.Interfaces;

namespace RedmineApp.Repositories.Implement
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly AppDbContext _context;
        public ProjectMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateRangeAsync(List<ProjectMember> projectMembers)
        {
            await _context.ProjectMembers.AddRangeAsync(projectMembers);
        }

        public void Delete(ProjectMember member)
        {
            _context.ProjectMembers.Remove(member);
        }

        public async Task<ProjectMember?> GetByProjectAndUserAsync(int projectId, int userId)
        {
            return await _context.ProjectMembers.FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        }


        public async Task<bool> IsUserInProjectAsync(int projectId, int userId)
        {
            return await _context.ProjectMembers.AnyAsync(pm => pm.ProjectId ==projectId && pm.UserId == userId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
