using Domains;

namespace Makeen._Planner.Duty_Service
{
    public class UpdateDutyCommand
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DeadLine { get; set; }
        public DutyCategory DutyCategory { get; set; }
        public PriorityCategory PriorityCategory { get; set; }
    }
}
