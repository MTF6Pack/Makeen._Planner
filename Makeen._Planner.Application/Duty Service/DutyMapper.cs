using Makeen.Planner.Domain.Domains;

namespace Makeen._Planner.Duty_Service
{
    public static class DutyMapper
    {
        public static Duty ToModel(this AddDutyCommand command)
        {
            Duty newduty = new(command.Name, command.DeadLine, command.DutyCategory, command.PriorityCategory, command.User);
            return newduty;
        }
    }
}