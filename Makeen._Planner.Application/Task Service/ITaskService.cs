using Domain;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace Makeen._Planner.Task_Service
{
    public interface ITaskService
    {
        Task<bool> AddTask(AddTaskCommand command, Guid userid);
        //Task<List<Domain.Task.Task>> GetObjectByName(string name);
        //Task<List<Domain.Task.Task>> GetAllUserTasks(Guid userid);
        Task RemoveTask(Guid taskid);
        Task UpdateTask(Guid taskid, UpdateTaskCommand command);
        //Task<List<Domain.Task.Task>?> GetAllTasks();
        Task<List<Domain.Task>> GetTheUserOrGroupTasksByCalander(DateTime? date, Guid userid, Guid? groupid, bool isGrouptask);
        Task Done(Guid taskid);
        Task Done(List<Guid>? tasksid, DateTime? date);
    }
}
