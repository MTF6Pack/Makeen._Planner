namespace Domain
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid? Userid { get; private set; }
        public Task? Task { get; private set; }
        public string? Message { get; private set; }
        public bool IsDelivered { get; private set; }
        public NotificationType Type { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime DeliveryTime { get; private set; }

        public Notification(Task? task, string? message, Guid? userid, NotificationType notificationType)
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
            Task = task;
            Message = message;
            Userid = userid;
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
