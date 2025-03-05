namespace Persistence
{
    public class TaskDapperDto
    {
        public string Status { get; set; } = string.Empty;
        public DateTime DeadLine { get; set; }
        public int FutureTasksCount { get; set; }
    }
}
