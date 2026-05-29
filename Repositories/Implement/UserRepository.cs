using Microsoft.EntityFrameworkCore;
using RedmineApp.Models.EntityModels;
using RedmineApp.Repositories.Interfaces;

namespace RedmineApp.Repositories.Implement
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Users.AnyAsync(u => u.Code == code);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Positions)
                .AsNoTracking().
                FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetLastUserAsync()
        {
            return await _context.Users.OrderByDescending(u => u.Code).FirstOrDefaultAsync();
        }

        public async Task<List<Position>> GetPositionsByIdsAsync(List<int> ids)
        {
            return await _context.Positions.Where(p => ids.Contains(p.Id)).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            Console.WriteLine("Id: " + id);
            return await _context.Users
                .Include (u => u.Positions)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserDetailByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Positions)
                .FirstOrDefaultAsync(u => u.Id == id && !u.DeleteFlg);
        }

        public async Task<List<User>> GetUsersNotInProjectAsync(int projectId)
        {
            return await _context.Users
                .Where(u => !u.DeleteFlg)
                .Where(u => !_context.ProjectMembers.Any(pm => pm.ProjectId == projectId && pm.UserId == u.Id))
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();  
        }
    }
}

