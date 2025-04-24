//using Domain.Events;
//using Domain;
//using MediatR;
//using Microsoft.EntityFrameworkCore.Diagnostics;
//using Microsoft.EntityFrameworkCore;
//using Persistence;

//namespace Persistence.Interceptors
//{
//    public class NotificationInterceptor(IMediator mediator) : SaveChangesInterceptor
//    {
//        private readonly IMediator _mediator = mediator;

//        public override async ValueTask<int> SavedChangesAsync(
//     SaveChangesCompletedEventData eventData,
//     int result,
//     CancellationToken cancellationToken = default)
//        {
//            Console.WriteLine("🟢 NotificationInterceptor is running...");

//            if (eventData.Context is not DataBaseContext dbContext)
//                return await base.SavedChangesAsync(eventData, result, cancellationToken);

//            // Log all tracked Notification entries and their state for debugging:
//            var entries = dbContext.ChangeTracker.Entries<Notification>().ToList();
//            Console.WriteLine($"[Interceptor] Total tracked Notification entries: {entries.Count}");
//            foreach (var entry in entries)
//            {
//                Console.WriteLine($"[Interceptor] Notification {entry.Entity.Id} state: {entry.State}");
//            }

//            // Now query for the newly added notifications (using AsEnumerable to force client-side evaluation)
//            var newNotifications = dbContext.ChangeTracker.Entries<Notification>()
//                .AsEnumerable()
//                .Where(e => e.State == EntityState.Added)
//                .Select(e => e.Entity)
//                .ToList();

//            if (newNotifications.Count == 0)
//            {
//                Console.WriteLine("🚫 No new notifications found!");
//            }
//            else
//            {
//                foreach (var notification in newNotifications)
//                {
//                    Console.WriteLine($"📢 New notification detected: {notification.Message}");
//                    var userId = notification.Userid;
//                    if (userId == null) continue;

//                    await _mediator.Publish(new NotificationCreatedEvent(userId.Value, notification.Message!), cancellationToken);
//                    Console.WriteLine($"📨 Published event for user {userId}");
//                }
//            }

//            return await base.SavedChangesAsync(eventData, result, cancellationToken);
//        }
//    }
//}