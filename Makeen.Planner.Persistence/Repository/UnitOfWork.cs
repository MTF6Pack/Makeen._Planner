using Persistence;

namespace Persistence.Repository
{
    public class UnitOfWork(DataBaseContext context) : IUnitOfWork
    {
        private readonly DataBaseContext _context = context;
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}