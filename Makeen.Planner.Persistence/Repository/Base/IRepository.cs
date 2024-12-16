using Microsoft.EntityFrameworkCore;

namespace Repository.Base
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        Task<T?> GetObjectByIdAsync(Guid id);
        T? GetObjectById(Guid id);
        DbSet<T> StraitAccess();
        Task<List<T>?> GetAll();
        void Delete(Guid id);
    }
}
