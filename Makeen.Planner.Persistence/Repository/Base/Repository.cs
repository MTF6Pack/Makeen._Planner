using Microsoft.EntityFrameworkCore;

namespace Persistence.Repository.Base
{
    public class Repository<T>(DataBaseContext context) : IRepository<T> where T : class
    {
        private readonly DbSet<T> _DbSet = context.Set<T>();

        DbContext IRepository<T>.StraitAccess { get => context; }

        public async Task Add(T entity)
        {
            await _DbSet.AddAsync(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _DbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _DbSet.Remove(entity);
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var t = _DbSet.FindAsync(id);
            return await t;
        }

        public async Task<List<T>?> GetAllAsync()
        {
            return await _DbSet.ToListAsync();
        }

        public async Task<T?> GetObjectByName(string name)
        {
            return await _DbSet.FindAsync(name);
        }
    }
}