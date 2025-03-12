namespace Makeen._Planner.Task_Service
{
    public static class TaskMapper
    {
        public static Domain.Task.Task ToModel(this AddTaskCommand command, Guid? senderId)
        {
            Domain.Task.Task newtask = new(

                command.GroupId,
                command.Name,
                command.DeadLine,
                command.PriorityCategory,
                command.StartTime,
                command.Repeat,
                command.Alarm,
                command.Description,
               senderId
                );

            return newtask;
        }
    }
}