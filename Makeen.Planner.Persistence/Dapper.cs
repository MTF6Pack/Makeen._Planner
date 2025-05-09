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
        public static async Task<Dictionary<string, List<NotificationDto>>> GetUserNotificationsAsync(Guid userId, bool beSorted)
        {
            string _connectionString = "Server=.;Database=Planner;Trusted_Connection=True;TrustServerCertificate=True;";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
            SELECT 
          n.Id,
          n.Message,
          n.ReceiverId,
          t.CreationTime,
          CASE WHEN t.GroupId IS NOT NULL THEN g.Title END AS GroupName,
          CASE WHEN t.GroupId IS NOT NULL THEN g.AvatarUrl END AS GroupPhoto,
          CASE WHEN t.GroupId IS NOT NULL THEN g.Color END AS GroupColor,
          t.Result,
          CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.AvatarUrl END AS SenderPhoto,
          CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.UserName END AS SenderUserName,
          CASE WHEN t.GroupId IS NULL AND t.SenderId IS NOT NULL THEN s.Fullname END AS SenderName
      FROM [Planner].[dbo].[Notifications] AS n
      LEFT JOIN [Planner].[dbo].[Tasks] AS t ON t.Id = n.TaskId
      LEFT JOIN [Planner].[dbo].[Groups] AS g ON t.GroupId = g.Id
      LEFT JOIN [dbo].[AspNetUsers] AS s ON t.SenderId = s.Id
      WHERE n.ReceiverId = @ReceiverId
      ORDER BY n.CreationTime DESC";

            var rawNotifications = (await connection.QueryAsync<RawNotificationDto>(query, new { ReceiverId = userId })).ToList();

            var notifications = rawNotifications.Select(raw =>
            {
                SenderInfo? senderInfo = null;
                if (!string.IsNullOrEmpty(raw.GroupName))
                {
                    senderInfo = new SenderInfo
                    {
                        SenderName = raw.GroupName ?? "",
                        SenderColor = raw.GroupColor,
                        SenderPhoto = raw.GroupPhoto
                    };
                }
                else if (!string.IsNullOrEmpty(raw.SenderUserName))
                {
                    senderInfo = new SenderInfo
                    {
                        SenderName = raw.SenderName ?? "",
                        SenderUsername = raw.SenderUserName ?? "",
                        SenderPhoto = raw.SenderPhoto ?? ""
                    };
                }
                return new NotificationDto
                {
                    Id = raw.Id,
                    Message = raw.Message,
                    CreationTime = DateHelper.ConvertGregorianToPersian(raw.CreationTime, true).persianDate,
                    Result = raw.Result,
                    SenderInfo = senderInfo
                };
            }).ToList();

            var resultDictionary = new Dictionary<string, List<NotificationDto>>();

            if (beSorted)
            {
                var completedTasks = notifications.Where(n => n.Result == "Completed").ToList();
                var failedTasks = notifications.Where(n => n.Result == "Failed").ToList();
                var upcomingTasks = notifications.Where(n => n.Result == "Upcoming").ToList();

                resultDictionary.Add("Completed", completedTasks);
                resultDictionary.Add("Failed", failedTasks);
                resultDictionary.Add("Upcoming", upcomingTasks);
            }
            else
            {
                resultDictionary.Add("Notifications", notifications);
            }

            return resultDictionary;
        }
    }
}