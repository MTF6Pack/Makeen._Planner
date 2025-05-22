using System.Runtime.CompilerServices;

namespace Domain
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid? SenderId { get; private set; }
        public Guid ReceiverId { get; private set; }
        public Task? Task { get; private set; }
        public string? Message { get; private set; }
        public bool IsDelivered { get; private set; }
        public NotificationType Type { get; private set; }
        public DateTime CreationTime { get; private set; }
        public int Snooze { get; private set; }

        public Notification(Task? task, string? message, NotificationType notificationType, Guid? senderId, Guid receiverId)
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
            Task = task;
            Message = message;
            SenderId = senderId;
            ReceiverId = receiverId;
            Type = notificationType;
            IsDelivered = false;
            Snooze = 0;
        }

        public void Deliver()
        {
            IsDelivered = true;
        }

        public void UnDeliver()
        {
            IsDelivered = false;
            Snooze = 0;
        }

        public void SetSnooze(int minute)
        {
            Snooze = minute;
            if (minute != 0) CreationTime = DateTime.Now.AddMinutes(minute);
        }
        public Notification()
        {

        }
    }
}
