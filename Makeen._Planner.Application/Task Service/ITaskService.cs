namespace Makeen._Planner.Task_Service
{
    public interface ITaskService
    {
        Task AddTask(AddTaskCommand command);
        Domain.Task.Task? GetObjectByName(string name);
        List<Domain.Task.Task> GetAllUserTasks(Guid id);
        void RemoveTask(Guid id);
        void UpdateTask(Guid id, UpdateTaskCommand command);
    }
}
