//using Domain.Events;
//using Domain;
//using MediatR;
//using Microsoft.EntityFrameworkCore.Diagnostics;
//using Microsoft.EntityFrameworkCore;
//using Persistence;

//namespace Persistence.Interceptors
//{
//    public class TaskNotificationInterceptor(IMediator mediator) : SaveChangesInterceptor
//    {
//        private readonly IMediator _mediator = mediator;

//        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
//        {
//            Console.WriteLine("🟢 NotificationInterceptor is running...");
//            if (eventData.Context is not DataBaseContext dbContext)
//                return await base.SavedChangesAsync(eventData, result, cancellationToken);

//            DateTime now = DateTime.Now;
//            var tasksToNotify = dbContext.ChangeTracker.Entries<Domain.Task>()
//                .Where(e => e.State == EntityState.Modified)
//                .Select(e => e.Entity)
//                .Where(t => t.Alarm.HasValue && t.StartTime.AddMinutes(-(int)t.Alarm.Value) <= now && t.Status != Domain.TaskEnums.Status.Done)
//                .ToList();

//            if (tasksToNotify.Count == 0)
//            {
//                Console.WriteLine("🚫 No new notifications found!");
//            }
//            else
//            {

//                foreach (var task in tasksToNotify)
//                {
//                    Console.WriteLine($"📢 New notification detected: {tasksToNotify}");
//                    if (task.User == null) continue;

//                    var notification = new Notification(task, "زمان فعالیت شما سر رسیده", task.User.Id, NotificationType.Reminder);
//                    dbContext.Notifications.Add(notification);
//                    task.User.Notifications!.Add(notification);
//                    Console.WriteLine($"🔔 Created Reminder Notification for User {task.User.Id}");

//                    // Publish the event instead of calling SignalR directly
//                    await _mediator.Publish(new TaskReminderEvent(task.User.Id, notification), cancellationToken);
//                    Console.WriteLine($"📢 Published NotificationCreatedEvent for {task.User.Id}");
//                }
//            }
//            return await base.SavedChangesAsync(eventData, result, cancellationToken);
//        }
//    }
//}