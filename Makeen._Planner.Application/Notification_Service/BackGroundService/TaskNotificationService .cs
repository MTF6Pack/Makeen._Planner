using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Notification_Service.BackGroundService
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
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

                    DateTime now = DateTime.Now;
                    var tasksToNotify = await dbContext.Tasks
    .Include(t => t.User)
    .Where(t => t.User != null &&
                t.StartTime.AddMinutes(-(int)t.Alarm!) <= now &&
                t.Status != Domain.Task.TaskStatus.Done)
    .ToListAsync(stoppingToken);

                    foreach (var task in tasksToNotify)
                    {
                        // Here, task contains all its properties, and task.User is fully loaded.
                        Guid userId = task.User!.Id;  // Access the user's Id directly
                        await PlantNotification(task, userId, dbContext);
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in TaskNotificationService: {ex.Message}", ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Run every 30 seconds
            }
        }

        private async Task PlantNotification(Domain.Task.Task task, Guid? userId, DataBaseContext dbContext)
        {
            string message = "زمان فعالیت شما سر رسیده";
            // Check if a notification already exists for this task
            bool alreadyNotified = await dbContext.Notifications
                .AnyAsync(n => n.Task!.Id == task.Id, CancellationToken.None);

            if (!alreadyNotified)
            {
                var user = await dbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {userId} not found. Skipping notification.", userId);
                    return;
                }

                Notification notification = new(task, message, userId);
                notification.Activate();

                user.Notifications?.Add(notification);
                dbContext.Notifications.Add(notification);

                _logger.LogInformation("🔔 Notification sent for Task '{task.Name}' to user {userId} at {DateTime.Now}.", task.Name, userId, DateTime.Now);
            }
        }
    }
}