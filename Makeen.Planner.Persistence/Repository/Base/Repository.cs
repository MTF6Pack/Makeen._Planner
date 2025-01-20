using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Persistence.Repository.Base
{
    public class Repository<T>(DataBaseContext context) : IRepository<T> where T : class
    {
        private readonly DbSet<T> _DbSet = context.Set<T>();
        public void Add(T entity)
        {
            _DbSet.Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _DbSet.AddAsync(entity);
        }

        public async void Delete(Guid id)
        {
            T? t = await GetByIdAsync(id);
            if (t != null) _DbSet.Remove(t);
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var user = _DbSet.FindAsync(id);
            return await user;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _DbSet.ToListAsync();
        }

        public async Task<T?> GetObjectByName(string name)
        {
            return await _DbSet.FindAsync(name);
        }

        public DbContext StraitAccess()
        {
            return context;
        }
    }
}