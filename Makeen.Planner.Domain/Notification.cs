namespace Domain
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid? Userid { get; private set; }
        public Task.Task? Task { get; private set; }
        public string? Message { get; private set; }
        public bool IsActivated { get; private set; }

        public Notification(Task.Task? task, string? message, Guid? userid)
        {
            Id = Guid.NewGuid();
            Task = task;
            Message = message;
            Userid = userid;
        }

        public void Activate()
        {
            IsActivated = true;
        }

        public Notification()
        {

        }
    }
}
