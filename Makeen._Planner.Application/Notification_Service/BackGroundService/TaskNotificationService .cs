using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
using Task = System.Threading.Tasks.Task;

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

                    // Fetch tasks that need notifications and ensure User is loaded
                    var tasksToNotify = await dbContext.Tasks
                        .Include(t => t.User)
                        .Where(t => t.User != null &&
                                    t.Alarm.HasValue &&
                                    t.StartTime.AddMinutes(-(int)t.Alarm.Value) <= now &&
                                    t.Status != Domain.TaskEnums.Status.Done)
                        .ToListAsync(stoppingToken);

                    foreach (var task in tasksToNotify)
                    {
                        if (task.User == null)
                        {
                            _logger.LogWarning("Task '{TaskName}' does not have an associated user. Skipping.", task.Name);
                            continue;
                        }

                        Guid userId = task.User.Id;  // Get User ID from navigation property
                        await SendNotification(task, userId, dbContext);
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in TaskNotificationService: {Message}", ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Run every 30 seconds
            }
        }

        private async Task SendNotification(Domain.Task task, Guid userId, DataBaseContext dbContext)
        {
            string message = "زمان فعالیت شما سر رسیده"; // "Your task time has arrived"

            // Check if a notification already exists for this task
            bool alreadyNotified = await dbContext.Notifications
                .AnyAsync(n => n.Task!.Id == task.Id, CancellationToken.None);

            if (!alreadyNotified)
            {
                var user = await dbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found. Skipping notification.", userId);
                    return;
                }

                Notification notification = new(task, message, userId);

                user.Notifications?.Add(notification);
                dbContext.Notifications.Add(notification);

                _logger.LogInformation("🔔 Notification sent for Task '{TaskName}' to user {UserId} at {Time}.",
                    task.Name, userId, DateTime.Now);
            }
        }
    }
}