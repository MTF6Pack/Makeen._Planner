namespace Domain.Services
{
    public interface INotificationSender
    {
        System.Threading.Tasks.Task SendNotificationAsync(string userId, object payload, CancellationToken cancellationToken = default);
    }
}