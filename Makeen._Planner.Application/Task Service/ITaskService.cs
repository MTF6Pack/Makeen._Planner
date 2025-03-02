using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Task_Service
{
    public interface ITaskService
    {
        Task AddTask(AddTaskCommand command, Guid userid);
        Task<List<Domain.Task.Task>> GetObjectByName(string name);
        Task<List<Domain.Task.Task>> GetAllUserTasks(Guid userid);
        Task RemoveTask(Guid taskid);
        Task UpdateTask(UpdateTaskCommand command, Guid userid);
        Task<List<Domain.Task.Task>?> GetAllTasks();
        Task<List<Domain.Task.Task>?> GetTheUserTasksByCalander(DateOnly date, Guid userid);
    }
}
