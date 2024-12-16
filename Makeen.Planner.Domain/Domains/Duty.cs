using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Domains
{
    public class Duty
    {
        public string? Name { get; private set; }
        public DateTime DeadLine { get; private set; }
        public DutyCategory DutyCategory { get; private set; }
        public PriorityCategory PriorityCategory { get; private set; }
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public User? User { get; private set; }

        public Duty(string name, DateTime deadline, DutyCategory dutyCategory, PriorityCategory priorityCategory, Guid userid)
        {
            Name = name;
            DeadLine = deadline;
            DutyCategory = dutyCategory;
            PriorityCategory = priorityCategory;
            UserId = userid;
        }

        public void UpdateDuty(string name, DateTime deadline, DutyCategory dutyCategory, PriorityCategory priorityCategory)
        {
            if (name != null) Name = name;
            DeadLine = deadline;
            DutyCategory = dutyCategory;
            PriorityCategory = priorityCategory;
        }
        private Duty()
        {
        }
    }
}
