namespace Makeen._Planner.Task_Service
{
    public interface ITaskService
    {
        Task AddTask(AddTaskCommand command);
        Domain.Task.Task? GetObjectByName(string name);
        Task<List<Domain.Task.Task>?> GetAllTasks();
        void RemoveTask(Guid id);
        void UpdateTask(Guid id, UpdateTaskCommand command);

    }
}
