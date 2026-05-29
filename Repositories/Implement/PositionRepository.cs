using Microsoft.EntityFrameworkCore;
using RedmineApp.Models.EntityModels;
using RedmineApp.Repositories.Interfaces;
namespace RedmineApp.Repositories.Implement
{
    public class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        private readonly AppDbContext _context;
        public PositionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Position>> GetAvailablePositionsForUserAsync(int userId)
        {
            var assignedPositionIds = await _context.Users
                .Where(u => u.Id == userId && !u.DeleteFlg)
                .SelectMany(u => u.Positions)
                .Select(p =>p.Id)
                .ToListAsync();
            return await _context.Positions
                .Where(p => !p.DeleteFlg && !assignedPositionIds.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<List<Position>> GetPositionsByUserIdAsync(int userId)
        {
            return await _context.Users
                .Where(u=> u.Id == userId && !u.DeleteFlg)
                .SelectMany(u => u.Positions)
                .Where(p => !p.DeleteFlg)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithPositionsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Positions)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.DeleteFlg);
        }
        public async Task<List<Position>> GetPositionsByIdsAsync(List<int> ids)
        {
            return await _context.Positions.Where(p => ids.Contains(p.Id)).ToListAsync();
        }
    }
}
