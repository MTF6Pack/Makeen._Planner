namespace Persistence.Repository
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
