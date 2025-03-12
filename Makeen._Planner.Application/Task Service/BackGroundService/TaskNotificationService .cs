using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Services
{
    public class TaskNotificationService(IServiceProvider serviceProvider, ILogger<TaskNotificationService> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<TaskNotificationService> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();  // ✅ Create Scope Here
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

                    DateTime now = DateTime.Now;
                    var tasksToNotify = dbContext.Tasks
                        .Where(t => t.DeadLine.AddMinutes(-(int)t.Alarm) <= now && t.Status != Domain.Task.TaskStatus.Done)
                        .ToList();

                    foreach (var task in tasksToNotify)
                    {
                        SendNotification(task);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in TaskNotificationService: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private void SendNotification(Domain.Task.Task task)
        {
            _logger.LogInformation($"🔔 Notification: Task '{task.Name}' is due at {task.DeadLine}.");
        }
    }
}