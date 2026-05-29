using Microsoft.EntityFrameworkCore.Storage;

namespace RedmineApp.Repositories.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T?>GetByIdAsync(int id);
        Task<bool>DeleteByIdAsync(int id);
        Task<int> CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task EndTransactionAsync();
        Task RollbackTransactionAsync();
        Task<bool> SaveChangesAsync();
    }
}
