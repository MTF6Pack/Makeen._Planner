namespace Domain.RepositoryInterfaces
{
    public interface IUserRepository : IRepository<User>
    {
        bool IsValidBase64(string base64String);
    }
}
