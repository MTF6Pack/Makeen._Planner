using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infrastructure.Date_and_Time;

public class PersianDateModelBinder : IModelBinder
{
    private static readonly PersianCalendar PersianCalendar = new();

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueResult == ValueProviderResult.None) return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
        string dateString = valueResult.FirstValue!;

        if (string.IsNullOrEmpty(dateString)) return Task.CompletedTask;

        try
        {
            DateTime persianDate = ParsePersianDate(dateString);
            bindingContext.Result = ModelBindingResult.Success(persianDate);
        }
        catch (Exception ex)
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Invalid date format: {ex.Message}");
        }

        return Task.CompletedTask;
    }

    private static DateTime ParsePersianDate(string dateStr)
    {
        string[] parts = dateStr.Split(' ');
        string datePart = parts[0];
        string timePart = parts.Length > 1 ? parts[1] : "0:0:0";

        string[] dateComponents = datePart.Split(['/', '-'], StringSplitOptions.RemoveEmptyEntries);
        if (dateComponents.Length < 3) throw new FormatException("The date part must contain year, month, and day.");

        int year = int.Parse(dateComponents[0]);
        int month = int.Parse(dateComponents[1]);
        int day = int.Parse(dateComponents[2]);

        int hour = 0, minute = 0, second = 0;
        if (!string.IsNullOrEmpty(timePart))
        {
            string[] timeComponents = timePart.Split(':');
            if (timeComponents.Length > 0) hour = int.Parse(timeComponents[0]);
            if (timeComponents.Length > 1) minute = int.Parse(timeComponents[1]);
            if (timeComponents.Length > 2) second = int.Parse(timeComponents[2]);
        }

        return PersianCalendar.ToDateTime(year, month, day, hour, minute, second, 0);
    }
}