using Microsoft.EntityFrameworkCore;

namespace Domain.RepositoryInterfaces;
public interface IRepository<T> where T : class
{
    System.Threading.Tasks.Task Add(T entity);
    System.Threading.Tasks.Task AddAsync(T entity);
    System.Threading.Tasks.Task<T?> GetByIdAsync(Guid id);
    DbContext StraitAccess { get; }
    System.Threading.Tasks.Task<List<T>?> GetAllAsync();
    void Delete(T entity);
}
