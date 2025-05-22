using Dapper;
using Domain.Helpers;
using Microsoft.Data.SqlClient;

namespace Persistence;

public static class Dapper
{
    public static async Task<Dictionary<string, object>> TasksReport(Guid id)
    {
        string connectionString = "Server=.;Database=Planner;Trusted_Connection=True;TrustServerCertificate=True;";
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        var today = DateTime.Now.Date;
        var userWeekReport = (await connection.QueryAsync<TaskDapperDto>(
            $"SELECT status, CONVERT(date, DeadLine) AS DeadLine, " +
            $"(SELECT COUNT(*) FROM [Planner].[dbo].[Tasks] " +
            $" WHERE DeadLine > GETDATE() " +
            $" AND (UserId = '{id}' OR GroupId = '{id}') " +
            $" AND Status = 'Pending') AS UpcomingTasksCount " +
            $"FROM [Planner].[dbo].[Tasks] " +
            $"WHERE (UserId = '{id}' OR GroupId = '{id}') and DeadLine BETWEEN DATEADD(DAY, -6, CAST(GETDATE() AS DATE)) And " +
            $"DATEADD(SECOND, -1, DATEADD(DAY, 1, CONVERT(DATETIME, CAST(GETDATE() AS DATE))))"
        )).ToList();

        // Aggregate week-level statistics
        int weekAllTasksCount = userWeekReport.Count;
        int weekAllCompletedTaskscount = userWeekReport.Count(x => x.Status == "Done");
        int weekPendingCount = weekAllTasksCount - weekAllCompletedTaskscount;
        int UpcomingTasksCount = userWeekReport.FirstOrDefault()?.UpcomingTasksCount ?? 0;
        string weekLabel = $"{weekAllCompletedTaskscount}/{weekAllTasksCount}";

        var barCharts = new List<Dictionary<string, object>>();

        for (int i = 0; i < 7; i++)
        {
            var Id = i + 1;
            var day = DateTime.Now.AddDays(-i).Date;
            var dayName = HelperMethods.GetPersianDayName(day.DayOfWeek);
            int totalTasks = userWeekReport.Count(x => x.DeadLine.Date == day);
            int completedTasks = userWeekReport.Count(x => x.DeadLine.Date == day && x.Status == "Done");
            string dayLabel = $"{completedTasks}/{totalTasks}";

            barCharts.Add(new Dictionary<string, object>
            {
        { "id", Id},
        { "Date", DateHelper.ConvertGregorianToPersian(day)},
        { "DayName", dayName },
        { "CompletedTasks", completedTasks },
        { "PendingTasks", totalTasks-completedTasks },
        { "Label", dayLabel }
    });
        }
        var linechart = new Dictionary<string, object>
        {
            { "AllTasks", weekAllTasksCount },
             { "CompletedTasks", weekAllCompletedTaskscount },
             { "PendingTasks", weekPendingCount },
            { "UpcomingTasks", UpcomingTasksCount },
        };
        var pieChart = new List<Dictionary<string, object>>
        {
            new() { { "id", 1 }, { "Done", weekAllCompletedTaskscount } },
            new() { { "id", 2 }, { "Pending", weekPendingCount } }
        };

        var result = new Dictionary<string, object>
{
    { "WeekLabel", weekLabel },
    { "BarCharts", barCharts  },
    { "LineChart", linechart },
    { "PieChart", pieChart },
};

        return result;
    }

    public class NotificationQueryService
    {
        private const string ConnectionString = "Server=.;Database=Planner;Trusted_Connection=True;TrustServerCertificate=True;";

        public static async Task<Dictionary<string, List<NotificationDto>>> GetUserNotificationsAsync(Guid userId, bool onlyUndelivered = false, bool beSorted = false)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            string sql = onlyUndelivered
          ? UndeliveredSql
          : beSorted ? SortedNotificationsSql : AllNotificationsSql;

            var rawList = (await connection.QueryAsync<RawNotificationDto>(sql, new { ReceiverId = userId }))
                .ToList();

            // Map raw DTOs to NotificationDto and sort by CreationTime desc
            var notifications = rawList
                .Select(MapToDto)
                .OrderByDescending(n => n.CreationTime)
                .ToList();

            return GroupResults(notifications, beSorted);
        }

        private const string UndeliveredSql = @"
UPDATE n
SET    IsDelivered = 1
OUTPUT 
    inserted.Id,
    inserted.Message,
    inserted.CreationTime,
    t.Result,
    -- group fields
    CASE WHEN t.GroupId IS NOT NULL THEN g.Title      END AS GroupName,
    CASE WHEN t.GroupId IS NOT NULL THEN g.AvatarUrl END AS GroupPhoto,
    CASE WHEN t.GroupId IS NOT NULL THEN g.Color     END AS GroupColor,
    -- sender fields
    CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.Fullname   END AS SenderName,
    CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.UserName   END AS SenderUserName,
    CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.AvatarUrl END AS SenderPhoto
FROM [Planner].[dbo].[Notifications] AS n
LEFT JOIN [Planner].[dbo].[Tasks]      AS t ON t.Id       = n.TaskId
LEFT JOIN [Planner].[dbo].[Groups]     AS g ON t.GroupId  = g.Id
LEFT JOIN dbo.AspNetUsers              AS s ON t.SenderId = s.Id
WHERE n.ReceiverId   = @ReceiverId
  AND n.IsDelivered = 0;";

        private const string AllNotificationsSql = @"
BEGIN TRANSACTION;

-- mark all as delivered
UPDATE n
SET    IsDelivered = 1
FROM [Planner].[dbo].[Notifications] AS n
WHERE n.ReceiverId   = @ReceiverId
  AND n.IsDelivered = 0;

-- select full payload
SELECT 
    n.Id,
    n.Message,
    n.CreationTime,
    t.Result,
    CASE WHEN t.GroupId IS NOT NULL THEN g.Title      END AS GroupName,
    CASE WHEN t.GroupId IS NOT NULL THEN g.AvatarUrl END AS GroupPhoto,
    CASE WHEN t.GroupId IS NOT NULL THEN g.Color     END AS GroupColor,
    CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.Fullname   END AS SenderName,
    CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.UserName   END AS SenderUserName,
    CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.AvatarUrl END AS SenderPhoto
FROM [Planner].[dbo].[Notifications] AS n
LEFT JOIN [Planner].[dbo].[Tasks]      AS t ON t.Id       = n.TaskId
LEFT JOIN [Planner].[dbo].[Groups]     AS g ON t.GroupId  = g.Id
LEFT JOIN dbo.AspNetUsers              AS s ON t.SenderId = s.Id
WHERE n.ReceiverId = @ReceiverId
ORDER BY t.CreationTime DESC;

COMMIT TRANSACTION;";

        private const string SortedNotificationsSql = @"
BEGIN TRANSACTION;

-- mark all as delivered except Request and Respond types
UPDATE n
SET    IsDelivered = 1
FROM [Planner].[dbo].[Notifications] AS n
WHERE n.ReceiverId = @ReceiverId
  AND n.IsDelivered = 0
  AND n.Type NOT IN ('Request', 'Respond');

-- select full payload (excluding Request and Respond)
SELECT 
    n.Id,
    n.Message,
    n.CreationTime,
    t.Result,
    CASE WHEN t.GroupId IS NOT NULL THEN g.Title      END AS GroupName,
    CASE WHEN t.GroupId IS NOT NULL THEN g.AvatarUrl END AS GroupPhoto,
    CASE WHEN t.GroupId IS NOT NULL THEN g.Color     END AS GroupColor,
    CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.Fullname   END AS SenderName,
    CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.UserName   END AS SenderUserName,
    CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.AvatarUrl END AS SenderPhoto
FROM [Planner].[dbo].[Notifications] AS n
LEFT JOIN [Planner].[dbo].[Tasks]      AS t ON t.Id       = n.TaskId
LEFT JOIN [Planner].[dbo].[Groups]     AS g ON t.GroupId  = g.Id
LEFT JOIN dbo.AspNetUsers              AS s ON t.SenderId = s.Id
WHERE n.ReceiverId = @ReceiverId AND n.Type NOT IN ('Request', 'Respond')
ORDER BY t.CreationTime DESC;

COMMIT TRANSACTION;";

        private static NotificationDto MapToDto(RawNotificationDto raw)
        {
            SenderInfo? sender = null;

            if (!string.IsNullOrEmpty(raw.GroupName))
            {
                sender = new SenderInfo
                {
                    SenderName = raw.GroupName,
                    SenderColor = raw.GroupColor,
                    SenderPhoto = raw.GroupPhoto
                };
            }
            else if (!string.IsNullOrEmpty(raw.SenderUserName))
            {
                sender = new SenderInfo
                {
                    SenderUsername = raw.SenderUserName,
                    SenderName = raw.SenderName,
                    SenderPhoto = raw.SenderPhoto
                };
            }

            return new NotificationDto
            {
                Id = raw.Id,
                Message = raw.Message,
                CreationTime = raw.CreationTime,
                Result = raw.Result,
                SenderInfo = sender
            };
        }

        private static Dictionary<string, List<NotificationDto>> GroupResults(List<NotificationDto> list, bool beSorted)
        {
            if (!beSorted)
            {
                return new Dictionary<string, List<NotificationDto>>
                {
                    ["Notifications"] = list
                };
            }

            else return new Dictionary<string, List<NotificationDto>>
            {
                ["Completed"] = [.. list.Where(x => x.Result == "Completed")],
                ["Failed"] = [.. list.Where(x => x.Result == "Failed")],
                ["Upcoming"] = [.. list.Where(x => x.Result == "Upcoming")],
            };
        }
    }
}