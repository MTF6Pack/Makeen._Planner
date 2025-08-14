using MediatR;

namespace Domain.Events
{
    public class TaskReminderEvent(Guid userId, Notification notification) : INotification
    {
        public Guid UserId { get; } = userId;
        public Notification Notification { get; } = notification;
    }
}
