using Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

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

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, errorMessage, isHandled) = GetExceptionDetails(exception);

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new { error = new { message = errorMessage } });
            await context.Response.WriteAsync(result);

            // Only log unhandled exceptions to the console.
            if (!isHandled)
            {
                LogToConsole(exception);
            }
            LogToFile(exception);
        }

        private static (int, string, bool) GetExceptionDetails(Exception exception)
        {
            var customAttr = (CustomExceptionAttribute?)Attribute.GetCustomAttribute(
                exception.GetType(), typeof(CustomExceptionAttribute));
            if (customAttr != null)
            {
                return (customAttr.StatusCode, exception.Message, true);
            }
            return (500, "Something went wrong. Please ensure your input data is valid and try again.", false);
        }

        private static void LogToConsole(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now} | Unhandled Exception: {exception.Message}");
            Console.WriteLine($"Trace:\n{exception.StackTrace}");
            Console.ResetColor();
        }

        private static void LogToFile(Exception exception)
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            string logFilePath = Path.Combine(logDirectory, "Errors.txt");
            string logMessage = $"{DateTime.Now} | {exception.Message}\n{exception.InnerException}\n" +
                                $"Trace:\n{exception.StackTrace}\n{new string('-', 100)}\n\n";
            File.AppendAllText(logFilePath, logMessage);
        }
    }
}