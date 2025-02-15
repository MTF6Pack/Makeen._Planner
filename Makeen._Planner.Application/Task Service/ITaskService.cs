namespace Makeen._Planner.Task_Service
{
    public interface ITaskService
    {
        Task AddTask(AddTaskCommand command);
        Task<Domain.Task.Task?> GetObjectByName(string name);
        Task<List<Domain.Task.Task>> GetAllUserTasks(Guid id);
        Task RemoveTask(Guid id);
        Task UpdateTask(Guid id, UpdateTaskCommand command);
    }
}
