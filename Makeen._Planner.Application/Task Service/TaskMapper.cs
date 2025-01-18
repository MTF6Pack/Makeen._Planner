namespace Makeen._Planner.Task_Service
{
    public static class TaskMapper
    {
        public static Domain.Task.Task ToModel(this AddTaskCommand command)
        {
            Domain.Task.Task newtask = new(

                command.UserId,
                command.GroupId,
                command.Name,
                command.DeadLine,
                command.IsInGroup,
                command.TaskCategory,
                command.PriorityCategory,
                command.Status
                );

            return newtask;
        }
    }
}