using Microsoft.EntityFrameworkCore;

namespace Persistence.Repository.Base
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        Task AddAsync(T entity);
        Task<T?> GetByIdAsync(Guid id);
        DbContext StraitAccess();
        Task<List<T>> GetAllAsync();
        void Delete(Guid id);
    }
}
