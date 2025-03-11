namespace Makeen._Planner.Task_Service
{
    public static class TaskMapper
    {
        public static Domain.Task.Task ToModel(this AddTaskCommand command)
        {
            Domain.Task.Task newtask = new(

                command.GroupId,
                command.Name,
                command.DeadLine,
                command.PriorityCategory,
                command.StartTime,
                command.Repeat,
                command.Alarm
                );

            return newtask;
        }

        public static Domain.Task.Task ToModel(this AddSendTaskCommand command, Guid senderid)
        {
            Domain.Task.Task newtask = new(

                command.Name,
                command.DeadLine,
                command.PriorityCategory,
                command.StartTime,
                senderid,
                command.Repeat,
                command.Alarm
                );

            return newtask;
        }
    }
}