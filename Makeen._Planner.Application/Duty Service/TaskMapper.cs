using Domains;

namespace Makeen._Planner.Task_Service
{
    public static class TaskMapper
    {
        public static Domains.Task ToModel(this AddTaskCommand command)
        {
            Domains.Task newtask = new(command.Name, command.DeadLine, command.TaskCategory, command.PriorityCategory, command.UserId);
            return newtask;
        }
    }
}