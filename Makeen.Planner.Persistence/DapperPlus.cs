//using Dapper;
//using Microsoft.Data.SqlClient;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Persistence
//{
//    public static class DapperPlus
//    {
//        public static async Task<Dictionary<string, object>> TasksReport(Guid id)
//        {
//            string connectionString = "Server=.;Database=Planner;Trusted_Connection=True;TrustServerCertificate=True;";

//            using var connection = new SqlConnection(connectionString);
//            await connection.OpenAsync();

//            var today = DateTime.Now.Date;
//            var daysAgo = Enumerable.Range(0, 7).Select(i => today.AddDays(-i)).ToList();

//            var userweekreport = (await connection.QueryAsync<Task>(
//                $"SELECT status, forMAT(CreationTime, 'yyyy-MM-dd') AS CreationTime FROM [Planner].[dbo].[Task] " +
//                $"WHERE (userid = @Id OR Groupid = @Id) AND " +
//                $"CreationTime >= @StartDate", new { Id = id, StartDate = daysAgo.Last() })
//            ).ToList();

//            var reportData = new Dictionary<string, object>
//        {
//            { nameof(weekAllTasksCount), userweekreport.Count },
//            { nameof(weekAllCompeletedTaskscount), userweekreport.Count(x => x.Status == ()1) },
//            { nameof(weekPendingCount), userweekreport.Count - userweekreport.Count(x => x.Status == (Domain.Task.TaskStatus)1) },
//            { nameof(weeklabel), $"{userweekreport.Count(x => x.Status == (Domain.Task.TaskStatus)1)}/{userweekreport.Count}" }
//        };

//            foreach (var day in daysAgo)
//            {
//                var dayLabel = day.ToString("yyyy-MM-dd");
//                var dayTasks = userweekreport.Where(x => x.CreationTime.Date == day).ToList();

//                reportData.Add($"{dayLabel}AllTaskscount", dayTasks.Count());
//                reportData.Add($"{dayLabel}AllCompeletedTaskscount", dayTasks.Count(x => x.Status == (Domain.Task.TaskStatus)1));
//                reportData.Add($"{dayLabel}Label", $"{dayTasks.Count(x => x.Status == (Domain.Task.TaskStatus)1)}/{dayTasks.Count()}");
//                reportData.Add($"{dayLabel}DayOfWeek", day.DayOfWeek);
//            }

//            return reportData;
//        }
//    }
