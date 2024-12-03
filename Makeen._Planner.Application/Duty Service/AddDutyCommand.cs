using Makeen.Planner.Domain.Domains;
using System.Runtime.CompilerServices;

namespace Makeen._Planner.Duty_Service
{
    public class AddDutyCommand
    {
        public required string Name { get; set; }
        public required DateTime DeadLine { get; set; }
        public required DutyCategory DutyCategory { get; set; }
        public required PriorityCategory PriorityCategory { get; set; }
        public required User User { get; set; }
    }
}
