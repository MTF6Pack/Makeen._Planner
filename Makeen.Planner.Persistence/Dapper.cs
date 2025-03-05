using Dapper;
using Domain;
using Domain.Task;
using Infrustucture;
using Microsoft.Data.SqlClient;
using System.Numerics;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using Task = Domain.Task.Task;

namespace Persistence;

public static class Dapper
{
    public static async Task<Dictionary<object, object>> TasksReport(Guid id)
    {
        string connectionString = "Server=.;Database=Planner;Trusted_Connection=True;TrustServerCertificate=True;";

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        var today = DateTime.Now.Date;
        var yesterday = DateTime.Now.AddDays(-1).Date;
        var twodaysago = DateTime.Now.AddDays(-2).Date;
        var threedaysago = DateTime.Now.AddDays(-3).Date;
        var fourdaysago = DateTime.Now.AddDays(-4).Date;
        var fivedaysago = DateTime.Now.AddDays(-5).Date;
        var sixdaysago = DateTime.Now.AddDays(-6).Date;

        var userweekreport = (await connection.QueryAsync<TaskDapperDto>($"SELECT status, Convert(date, DeadLine) AS DeadLine," +
            $"(SELECT COUNT(*) FROM[Planner].[dbo].[Task] WHERE DeadLine > '{today:yyyy-MM-dd}'" +
            $"AND(userid = '{id}' OR groupid = '{id}')" +
            $"AND Status = 'Pending') AS FutureTasksCount FROM[Planner].[dbo].[Task]" +
            $"WHERE DeadLine BETWEEN '{sixdaysago:yyyy-MM-dd}' AND '{today.AddDays(1):yyyy-MM-dd}'")).ToList();

        int weekAllTasksCount = userweekreport.Count;
        int weekAllCompeletedTaskscount = userweekreport.Count(x => x.Status == "Done");
        int weekPendingCount = weekAllTasksCount - weekAllCompeletedTaskscount;
        var firstReport = userweekreport.FirstOrDefault();
        int allremainingFutureTasksCount = firstReport != null ? firstReport.FutureTasksCount : 0;
        string weeklabel = $"{weekAllCompeletedTaskscount}/{weekAllTasksCount}";

        var todayDayOfWeek = HelperMethods.GetPersianDayName(today.DayOfWeek);
        int todayAllTaskscount = userweekreport.Count(x => x.DeadLine.Date == today);
        int todayAllCompeletedTaskscount = userweekreport.Count(x => x.DeadLine.Date == today && x.Status == "Done");
        string todayLabel = $"{todayAllCompeletedTaskscount}/{todayAllTaskscount}";

        var yesterdayDayOfWeek = HelperMethods.GetPersianDayName(yesterday.DayOfWeek);
        int yesterdayAllTaskscount = userweekreport.Count(x => x.DeadLine.Date == yesterday);
        int yesterdayAllCompeletedTaskscount = userweekreport.Count(x => x.DeadLine.Date == yesterday && x.Status == "Done");
        string yesterdayLabel = $"{yesterdayAllCompeletedTaskscount}/{yesterdayAllTaskscount}";

        var twodaysagoDayOfWeek = HelperMethods.GetPersianDayName(twodaysago.DayOfWeek);
        int twodaysagoAllTaskscount = userweekreport.Count(x => x.DeadLine.Date == twodaysago);
        int twodaysagoAllCompeletedTaskscount = userweekreport.Count(x => x.DeadLine.Date == twodaysago && x.Status == "Done");
        string twodaysagoLabel = $"{twodaysagoAllCompeletedTaskscount}/{twodaysagoAllTaskscount}";


        var threedaysagoDayOfWeek = HelperMethods.GetPersianDayName(threedaysago.DayOfWeek);
        int threedaysagoAllTaskscount = userweekreport.Count(x => x.DeadLine.Date == threedaysago);
        int threedaysagoAllCompeletedTaskscount = userweekreport.Count(x => x.DeadLine.Date == threedaysago && x.Status == "Done");
        string threedaysagoLabel = $"{threedaysagoAllCompeletedTaskscount}/{threedaysagoAllTaskscount}";

        var fourdaysagoDayOfWeek = HelperMethods.GetPersianDayName(fourdaysago.DayOfWeek);
        int fourdaysagoAllTaskscount = userweekreport.Count(x => x.DeadLine.Date == fourdaysago);
        int fourdaysagoAllCompeletedTaskscount = userweekreport.Count(x => x.DeadLine.Date == fourdaysago && x.Status == "Done");
        string fourdaysagoLabel = $"{fourdaysagoAllCompeletedTaskscount}/{fourdaysagoAllTaskscount}";

        var fivedaysagoDayOfWeek = HelperMethods.GetPersianDayName(fivedaysago.DayOfWeek);
        int fivedaysagoAllTaskscount = userweekreport.Count(x => x.DeadLine.Date == fivedaysago);
        int fivedaysagoAllCompeletedTaskscount = userweekreport.Count(x => x.DeadLine.Date == fivedaysago && x.Status == "Done");
        string fivedaysagoLabel = $"{fivedaysagoAllCompeletedTaskscount}/{fivedaysagoAllTaskscount}";

        var sixdaysagoDayOfWeek = HelperMethods.GetPersianDayName(sixdaysago.DayOfWeek);
        int sixdaysagoAllTaskscount = userweekreport.Count(x => x.DeadLine.Date == sixdaysago);
        int sixdaysagoAllCompeletedTaskscount = userweekreport.Count(x => x.DeadLine.Date == sixdaysago && x.Status == "Done");
        string sixdaysagoLabel = $"{sixdaysagoAllCompeletedTaskscount}/{sixdaysagoAllTaskscount}";

        Dictionary<object, object> myDictionary = [];
        myDictionary.Add(nameof(weekAllTasksCount), weekAllTasksCount);
        myDictionary.Add(nameof(weekAllCompeletedTaskscount), weekAllCompeletedTaskscount);
        myDictionary.Add(nameof(weekPendingCount), weekPendingCount);
        myDictionary.Add(nameof(allremainingFutureTasksCount), allremainingFutureTasksCount);
        myDictionary.Add(nameof(weeklabel), weeklabel);
        myDictionary.Add(nameof(todayAllTaskscount), todayAllTaskscount);
        myDictionary.Add(nameof(todayAllCompeletedTaskscount), todayAllCompeletedTaskscount);
        myDictionary.Add(nameof(todayLabel), todayLabel);
        myDictionary.Add(nameof(todayDayOfWeek), todayDayOfWeek);
        myDictionary.Add(nameof(yesterdayAllTaskscount), yesterdayAllTaskscount);
        myDictionary.Add(nameof(yesterdayAllCompeletedTaskscount), yesterdayAllCompeletedTaskscount);
        myDictionary.Add(nameof(yesterdayLabel), yesterdayLabel);
        myDictionary.Add(nameof(yesterdayDayOfWeek), yesterdayDayOfWeek);
        myDictionary.Add(nameof(twodaysagoAllTaskscount), twodaysagoAllTaskscount);
        myDictionary.Add(nameof(twodaysagoAllCompeletedTaskscount), twodaysagoAllCompeletedTaskscount);
        myDictionary.Add(nameof(twodaysagoLabel), twodaysagoLabel);
        myDictionary.Add(nameof(twodaysagoDayOfWeek), twodaysagoDayOfWeek);
        myDictionary.Add(nameof(threedaysagoAllTaskscount), threedaysagoAllTaskscount);
        myDictionary.Add(nameof(threedaysagoAllCompeletedTaskscount), threedaysagoAllCompeletedTaskscount);
        myDictionary.Add(nameof(threedaysagoLabel), threedaysagoLabel);
        myDictionary.Add(nameof(threedaysagoDayOfWeek), threedaysagoDayOfWeek);
        myDictionary.Add(nameof(fourdaysagoAllTaskscount), fourdaysagoAllTaskscount);
        myDictionary.Add(nameof(fourdaysagoAllCompeletedTaskscount), fourdaysagoAllCompeletedTaskscount);
        myDictionary.Add(nameof(fourdaysagoLabel), fourdaysagoLabel);
        myDictionary.Add(nameof(fourdaysagoDayOfWeek), fourdaysagoDayOfWeek);
        myDictionary.Add(nameof(fivedaysagoAllTaskscount), fivedaysagoAllTaskscount);
        myDictionary.Add(nameof(fivedaysagoAllCompeletedTaskscount), fivedaysagoAllCompeletedTaskscount);
        myDictionary.Add(nameof(fivedaysagoLabel), fivedaysagoLabel);
        myDictionary.Add(nameof(fivedaysagoDayOfWeek), fivedaysagoDayOfWeek);
        myDictionary.Add(nameof(sixdaysagoAllTaskscount), sixdaysagoAllTaskscount);
        myDictionary.Add(nameof(sixdaysagoAllCompeletedTaskscount), sixdaysagoAllCompeletedTaskscount);
        myDictionary.Add(nameof(sixdaysagoLabel), sixdaysagoLabel);
        myDictionary.Add(nameof(sixdaysagoDayOfWeek), sixdaysagoDayOfWeek);

        return myDictionary;
    }

    public static async Task<List<Task>> GroupTodayTasks(Guid groupid)
    {
        string connectionString = "Server=.;Database=Planner;Trusted_Connection=True;TrustServerCertificate=True;";

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        var todaygrouptasks = (await connection.QueryAsync<Task>($"SELECT [UserId],[Name],[Status],[DeadLine],[CreationTime]," +
            $"[PriorityCategory] FROM[Planner].[dbo].[Task] where(GroupId = '{groupid}'" +
            $" and CAST(DeadLine as date) = '{DateTime.Now.Date}')")).ToList();

        return todaygrouptasks;
    }
}