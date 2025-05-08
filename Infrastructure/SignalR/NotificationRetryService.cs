using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.SignalR
{
    public class NotificationRetryService(
        IServiceProvider services,
        ILogger<NotificationRetryService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var scope = services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
                    var sender = scope.ServiceProvider.GetRequiredService<NotificationSenderHandler>();

                    var pending = await db.QueuedNotifications.Where(q => q.RetryCount < 5).ToListAsync(ct);

                    foreach (var queued in pending)
                    {
                        //await sender.ResendQueuedNotification(queued, ct);
                        await Task.Delay(1000, ct); // Rate limiting
                    }

                    await Task.Delay(TimeSpan.FromMinutes(5), ct);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in notification retry service");
                }
            }
        }
    }
}