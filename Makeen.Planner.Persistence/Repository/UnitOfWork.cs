using Makeen.Planner.Persistence;

namespace Repository
{
    public class UnitOfWork(DataBaseContext context) : IUnitOfWork
    {
        private readonly DataBaseContext _context = context;
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
