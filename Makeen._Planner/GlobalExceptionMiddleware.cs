using Microsoft.AspNetCore.Mvc.Filters;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;

namespace Makeen._Planner
{
    public class GlobalExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, errorMessage) = GetExceptionDetails(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new { error = new { Message = errorMessage } });

            return context.Response.WriteAsync(result);
        }

        private static (int, string) GetExceptionDetails(Exception exception)
        {
            var customExceptionAttribute = (CustomExceptionAttribute?)Attribute.GetCustomAttribute(
                exception.GetType(), typeof(CustomExceptionAttribute));

            if (customExceptionAttribute != null)
                return (customExceptionAttribute.StatusCode, exception.Message);

            LogError(exception);  // Log only when needed
            return (400, "Something went wrong, Make sure the input data is valid then try again");
        }

        private static void LogError(Exception exception)
        {
            var logMessage = $"{DateTime.Now} | {exception.Message}\n{exception.InnerException}\n\nTrace:\n\n{exception.StackTrace}\n{new string('-', 100)}\n\n";
            File.AppendAllText("Logs/Errors.txt", logMessage);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(logMessage);
            Console.ResetColor();
        }
    }
}