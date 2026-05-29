using RedmineApp.Models.EntityModels;
using RedmineApp.Services.Interfaces;

namespace RedmineApp.Services.Implement
{
    public class CommonService : ICommonService
    {
        private readonly AppDbContext _context;
        private int _userId;
        public string? _userPosition;
        public CommonService(AppDbContext context)
        {
            _context = context;
        }

        public string? GetPosition()
        {
            return _userPosition;
        }

        public int GetUserId()
        {
            return _userId;
        }

        public void SetPosition(string position)
        {
            _userPosition = position;
        }

        public void SetUserId(int userId)
        {
            _userId = userId;
        }
    }
}
