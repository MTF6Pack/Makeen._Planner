using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Makeen.Planner.Domain.Domains
{
    public class Duty
    {
        public string? Name { get; private set; }
        public DateTime DeadLine { get; private set; }
        public DutyCategory DutyCategory { get; private set; }
        public PriorityCategory PriorityCategory { get; private set; }
        public Guid Id { get; private set; } = Guid.NewGuid();
        public User User { get; private set; }

        public Duty(string name, DateTime deadline, DutyCategory dutyCategory, PriorityCategory priorityCategory, User user)
        {
            Name = name;
            DeadLine = deadline;
            DutyCategory = dutyCategory;
            PriorityCategory = priorityCategory;
            User = user;
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
