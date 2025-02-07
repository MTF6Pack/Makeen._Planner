using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Domain.Task
{
    public class Task
    {
        public Guid Id { get; private set; }
        public User? User { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Guid? GroupId { get; private set; }
        public bool IsInGroup { get; private set; }
        public TaskStatus Status { get; private set; }
        public DateTime DeadLine { get; private set; }
        public DateTime CreationTime { get; private set; }
        public TaskCategory TaskCategory { get; private set; }
        public PriorityCategory PriorityCategory { get; private set; }

        public Task(Guid? groupId, string name, DateTime deadLine,
            TaskCategory taskCategory, PriorityCategory priorityCategory, TaskStatus status)
        {
            Name = name;
            Status = status;
            GroupId = groupId;
            Id = Guid.NewGuid();
            DeadLine = deadLine;
            TaskCategory = taskCategory;
            CreationTime = DateTime.Now;
            PriorityCategory = priorityCategory;
            IsInGroup = groupId != null;
        }
        public void UpdateTask(string name, DateTime deadline, TaskCategory taskCategory, PriorityCategory priorityCategory)
        {
            if (name != null) Name = name;
            DeadLine = deadline;
            TaskCategory = taskCategory;
            PriorityCategory = priorityCategory;
        }

        public void UpdateTaskStatus(TaskStatus status)
        {
            Status = status;
        }
        public Task()
        {
        }
    }
}
