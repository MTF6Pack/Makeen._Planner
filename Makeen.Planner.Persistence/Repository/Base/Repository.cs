using Makeen.Planner.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Repository.Base
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
            T? t = await GetObjectByIdAsync(id);
            if (t != null) _DbSet.Remove(t);
        }

        public async Task<T?> GetObjectByIdAsync(Guid id)
        {
            var user = _DbSet.FindAsync(id);
            return await user;
        }

        public async Task<List<T>?> GetAll()
        {
            return await _DbSet.ToListAsync();
        }

        public async Task<T?> GetObjectByName(string name)
        {
            return await _DbSet.FindAsync(name);
        }

        public T? GetObjectById(Guid id)
        {
            return _DbSet.Find(id);
        }

        public DbSet<T> StraitAccess()
        {
            return _DbSet;
        }
    }
}