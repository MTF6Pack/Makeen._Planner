namespace Domain
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid? Userid { get; private set; }
        public Domain.Task? Task { get; private set; }
        public string? Message { get; private set; }
        public bool IsDelivered { get; private set; }

        public Notification(Domain.Task? task, string? message, Guid? userid)
        {
            Id = Guid.NewGuid();
            Task = task;
            Message = message;
            Userid = userid;
        }

        public void Deliver()
        {
            IsDelivered = true;
        }

        public Notification()
        {

        }
    }
}
