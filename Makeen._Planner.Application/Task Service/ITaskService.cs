using Microsoft.AspNetCore.Mvc;

namespace Makeen._Planner.Task_Service
{
    public interface ITaskService
    {
        Task AddTask(AddTaskCommand command, [FromHeader] string token);
        Task<List<Domain.Task.Task>> GetObjectByName(string name);
        Task<List<Domain.Task.Task>> GetAllUserTasks(string token);
        Task RemoveTask(Guid taskid);
        Task UpdateTask(UpdateTaskCommand command, string token);
        Task<List<Domain.Task.Task>?> GetAllTasks();
        Task<List<Domain.Task.Task>?> GetTheUserTasksByCalander(DateOnly date, string token);
    }
}
