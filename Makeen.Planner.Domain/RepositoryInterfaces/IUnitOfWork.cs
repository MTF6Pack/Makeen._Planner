namespace Domain.RepositoryInterfaces;
public interface IUnitOfWork
{
    System.Threading.Tasks.Task SaveChangesAsync();
}
