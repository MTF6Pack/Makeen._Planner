using Domain;
using Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
using Task = System.Threading.Tasks.Task;

namespace Application.Notification_Service.BackGroundServices;

public class TaskReminderBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<TaskReminderBackgroundService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<TaskReminderBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Task Reminder Background Service Started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                DateTime now = DateTime.Now;
                var tasksToNotify = await dbContext.Tasks.Include(t => t.User)
                    .Where(t => t.User != null && t.Alarm.HasValue && now.AddMinutes(-(int)t.Alarm!.Value) <= t.StartTime && t.Status == Domain.TaskEnums.Status.Pending && t.Result != "Failed")
                    .ToListAsync(stoppingToken);

                foreach (var task in tasksToNotify)
                {
                    if (task.User == null)
                    {
                        continue;
                    }

                    bool reminderExists = await dbContext.Notifications.AnyAsync(n => n.Task!.Id == task.Id && n.Type == NotificationType.Reminder, stoppingToken);

                    if (reminderExists)
                    {
                        continue;
                    }

                    var notification = new Notification(task, "زمان فعالیت شما سر رسیده", NotificationType.Reminder, task.SenderId, task.User.Id);
                    dbContext.Notifications.Add(notification);

                    await dbContext.SaveChangesAsync(stoppingToken);

                    await mediator.Publish(new TaskReminderEvent(task.User.Id, notification), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in TaskReminderBackgroundService");
            }

            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken); // ⏳ Adjust the interval as needed
        }
    }
}