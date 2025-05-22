using Domain;
using Domain.Events;
using Domain.Helpers;
using Domain.TaskEnums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
using Task = System.Threading.Tasks.Task;

namespace Application.Notification_Service.BackGroundServices;

public class TaskReminderBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<TaskReminderBackgroundService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<TaskReminderBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Task Reminder Background Service Started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

                DateTime now = DateTime.Now;
                var tasksToNotify = await dbContext.Tasks.Include(t => t.User)
                    .Where(t => t.User != null && t.Alarm.HasValue && now.AddMinutes(-(int)t.Alarm!.Value) <= t.StartTime && t.Status == Status.Pending && t.Result != "Failed")
                    .ToListAsync(stoppingToken);

                foreach (var task in tasksToNotify)
                {
                    if (task.ReminderDismissed) continue;
                    bool reminderExists = await dbContext.Notifications.AnyAsync(n => n.Task!.Id == task.Id && n.Type == NotificationType.Reminder, stoppingToken);
                    if (reminderExists) continue;
                    if (task.Alarm!.Value != Alarm.None)
                    {
                        var notification = new Notification(task, $"تسک شما {HelperMethods.TranslateAlarm(task.Alarm.Value)} دیگر شروع می‌شود", NotificationType.Reminder, task.SenderId, task.User!.Id);
                        dbContext.Notifications.Add(notification);
                    }
                }

                var taskNotifsToSnooze = await dbContext.Notifications.Include(n => n.Task).Where(n => n.Snooze != 0 && n.CreationTime <= now).ToListAsync(cancellationToken: stoppingToken);

                foreach (var notif in taskNotifsToSnooze)
                {
                    notif.UnDeliver();
                }

                await dbContext.SaveChangesAsync(stoppingToken);

            }
            catch (Exception ex) { _logger.LogError(ex, "❌ Error in TaskReminderBackgroundService"); }

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}