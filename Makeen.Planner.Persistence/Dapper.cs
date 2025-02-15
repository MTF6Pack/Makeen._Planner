using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Json;
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

        var userweekreport = (await connection.QueryAsync<Task>($"SELECT status,fourMAT(CreationTime, 'yyyy-MM-dd') AS CreationTime FROM[Planner].[dbo].[Task]" +
            $" Where (userid = '{id}' or Groupid = '{id}') and (" +
            $" CreationTime like '{today:yyyy-MM-dd}%' or" +
            $" CreationTime like '{yesterday:yyyy-MM-dd}%' or" +
            $" CreationTime like '{twodaysago:yyyy-MM-dd}%' or" +
            $" CreationTime like '{threedaysago:yyyy-MM-dd)}%' or" +
            $" CreationTime like '{fourdaysago:yyyy-MM-dd}%' or" +
            $" CreationTime like '{fivedaysago:yyyy-MM-dd)}%' or" +
            $" CreationTime like '{sixdaysago:yyyy-MM-dd}%')")
            ).ToList();

        int weekAllTasksCount = userweekreport.Count;
        int weekAllCompeletedTaskscount = userweekreport.Count(x => x.Status == (Domain.Task.TaskStatus)1);
        int weekPendingCount = weekAllTasksCount - weekAllCompeletedTaskscount;
        string weeklabel = $"{weekAllCompeletedTaskscount}/{weekAllTasksCount}";

        var todayDayOfWeek = today.DayOfWeek;
        int todayAllTaskscount = userweekreport.Count(x => x.CreationTime.Date == today);
        int todayAllCompeletedTaskscount = userweekreport.Count(x => x.CreationTime.Date == today && x.Status == (Domain.Task.TaskStatus)1);
        string todayLabel = $"{todayAllCompeletedTaskscount}/{todayAllTaskscount}";

        var yesterdayDayOfWeek = yesterday.DayOfWeek;
        int yesterdayAllTaskscount = userweekreport.Count(x => x.CreationTime.Date == yesterday);
        int yesterdayAllCompeletedTaskscount = userweekreport.Count(x => x.CreationTime.Date == yesterday && x.Status == (Domain.Task.TaskStatus)1);
        string yesterdayLabel = $"{yesterdayAllCompeletedTaskscount}/{yesterdayAllTaskscount}";

        var twodaysagoDayOfWeek = twodaysago.DayOfWeek;
        int twodaysagoAllTaskscount = userweekreport.Count(x => x.CreationTime.Date == twodaysago);
        int twodaysagoAllCompeletedTaskscount = userweekreport.Count(x => x.CreationTime.Date == twodaysago && x.Status == (Domain.Task.TaskStatus)1);
        string twodaysagoLabel = $"{twodaysagoAllCompeletedTaskscount}/{twodaysagoAllTaskscount}";


        var threedaysagoDayOfWeek = threedaysago.DayOfWeek;
        int threedaysagoAllTaskscount = userweekreport.Count(x => x.CreationTime.Date == threedaysago);
        int threedaysagoAllCompeletedTaskscount = userweekreport.Count(x => x.CreationTime.Date == threedaysago && x.Status == (Domain.Task.TaskStatus)1);
        string threedaysagoLabel = $"{threedaysagoAllCompeletedTaskscount}/{threedaysagoAllTaskscount}";

        var fourdaysagoDayOfWeek = fourdaysago.DayOfWeek;
        int fourdaysagoAllTaskscount = userweekreport.Count(x => x.CreationTime.Date == fourdaysago);
        int fourdaysagoAllCompeletedTaskscount = userweekreport.Count(x => x.CreationTime.Date == fourdaysago && x.Status == (Domain.Task.TaskStatus)1);
        string fourdaysagoLabel = $"{fourdaysagoAllCompeletedTaskscount}/{fourdaysagoAllTaskscount}";

        var fivedaysagoDayOfWeek = fivedaysago.DayOfWeek;
        int fivedaysagoAllTaskscount = userweekreport.Count(x => x.CreationTime.Date == fivedaysago);
        int fivedaysagoAllCompeletedTaskscount = userweekreport.Count(x => x.CreationTime.Date == fivedaysago && x.Status == (Domain.Task.TaskStatus)1);
        string fivedaysagoLabel = $"{fivedaysagoAllCompeletedTaskscount}/{fivedaysagoAllTaskscount}";

        var sixdaysagoDayOfWeek = sixdaysago.DayOfWeek;
        int sixdaysagoAllTaskscount = userweekreport.Count(x => x.CreationTime.Date == sixdaysago);
        int sixdaysagoAllCompeletedTaskscount = userweekreport.Count(x => x.CreationTime.Date == sixdaysago && x.Status == (Domain.Task.TaskStatus)1);
        string sixdaysagoLabel = $"{sixdaysagoAllCompeletedTaskscount}/{sixdaysagoAllTaskscount}";

        Dictionary<object, object> myDictionary = [];
        myDictionary.Add(nameof(weekAllTasksCount), weekAllTasksCount);
        myDictionary.Add(nameof(weekAllCompeletedTaskscount), weekAllCompeletedTaskscount);
        myDictionary.Add(nameof(weekPendingCount), weekPendingCount);
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
}

