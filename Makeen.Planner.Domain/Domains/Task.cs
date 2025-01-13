using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Domains
{
    public class Task
    {
        public string? Name { get; private set; }
        public DateTime DeadLine { get; private set; }
        public TaskCategory TaskCategory { get; private set; }
        public PriorityCategory PriorityCategory { get; private set; }
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public User? User { get; private set; }

        public Task(string name, DateTime deadline, TaskCategory taskCategory, PriorityCategory priorityCategory, Guid userid)
        {
            Name = name;
            DeadLine = deadline;
            TaskCategory = taskCategory;
            PriorityCategory = priorityCategory;
            UserId = userid;
        }

        public void UpdateTask(string name, DateTime deadline, TaskCategory taskCategory, PriorityCategory priorityCategory)
        {
            if (name != null) Name = name;
            DeadLine = deadline;
            TaskCategory = taskCategory;
            PriorityCategory = priorityCategory;
        }
        public Task()
        {
        }
    }
}
