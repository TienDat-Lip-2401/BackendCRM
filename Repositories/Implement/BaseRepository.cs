using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RedmineApp.Models.EntityModels;
using RedmineApp.Repositories.Interfaces;

namespace RedmineApp.Repositories.Implement
{
    public class BaseRepository<T> : IBaseRepository<T> where T : EntityBaseModel
    {
        private readonly AppDbContext _context;
        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> CreateAsync(T entity)
        {
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            entity.DeleteFlg = false;

            await _context.Set<T>().AddAsync(entity);
            return entity.Id;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            int rowsAffected = await _context.Set<T>()
        .Where(x => x.Id == id)
        .ExecuteUpdateAsync(s => s.SetProperty(p => p.DeleteFlg, true)
                                  .SetProperty(p => p.UpdatedAt, DateTime.Now));

            return rowsAffected > 0;
        }

        public async Task EndTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().Where(x => !x.DeleteFlg).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().Where(x => x.Id == id && !x.DeleteFlg).FirstOrDefaultAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public Task UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.Now;
            var trackedEntity = _context.Set<T>().Local.FirstOrDefault(x => x.Id == entity.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                _context.Set<T>().Update(entity);
            }
            return Task.CompletedTask;
        }
    }
}
