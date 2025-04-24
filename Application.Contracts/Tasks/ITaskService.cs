using Application.Contracts.Tasks.Commands;
using Task = System.Threading.Tasks.Task;

namespace Application.Contracts.Tasks
{
    public interface ITaskService
    {
        Task<bool> AddTask(AddTaskCommand command, Guid userid);
        Task RemoveTask(Guid taskid);
        Task UpdateTask(Guid taskid, UpdateTaskCommand command);
        Task<List<Domain.Task>> GetTheUserOrGroupTasksByCalander(DateTime? date, Guid userid, Guid? groupid, bool isGrouptask);
        Task Done(Guid taskid);
        Task Done(List<Guid>? tasksid, DateTime? date);
        Task<List<Domain.Task>> GetAdminSentTasks(DateTime? date, Guid userid, Guid groupid);
    }
}
