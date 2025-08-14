using MediatR;

namespace Domain.Events
{
    public class NotificationCreatedEvent(Guid userId, string message) : INotification
    {
        public Guid UserId { get; } = userId;
        public string Message { get; } = message;
    }
}
