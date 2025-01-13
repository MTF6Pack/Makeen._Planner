using Domains;

namespace Makeen._Planner.Task_Service
{
    public interface ITaskService
    {
        Domains.Task? GetObjectByName(string name);
        Task<List<Domains.Task>?> GetAllTasks();
        void RemoveTask(Guid id);
        void UpdateTask(Guid id, UpdateTaskCommand command);

    }
}
