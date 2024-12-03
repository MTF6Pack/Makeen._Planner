namespace Makeen.Planner.Persistence.Repository
{
    public class UnitOfWork(DataBaseContext context) : IUnitOfWork
    {
        private readonly DataBaseContext context = context;

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
