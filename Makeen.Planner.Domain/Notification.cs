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
        public DateTime DeliveryTime { get; private set; }

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
        }

        public void Deliver()
        {
            IsDelivered = true;
            DeliveryTime = DateTime.Now;
        }

        public Notification()
        {

        }
    }
}
