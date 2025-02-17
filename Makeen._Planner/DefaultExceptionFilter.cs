using Microsoft.AspNetCore.Mvc.Filters;
using Infrustucture;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;

namespace Makeen._Planner
{
    public class DefaultExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var StatusCode = 400;
            var Error = "Something went wrong, Make sure the input data is valid then try again";

            var exceptionType = context.Exception.GetType();
            var customExceptionAttribute = (CustomExceptionAttribute?)Attribute.GetCustomAttribute(
            exceptionType, typeof(CustomExceptionAttribute));

            if (customExceptionAttribute != null)
            {
                StatusCode = customExceptionAttribute.StatusCode;
                Error = context.Exception.Message;
            }
            else
            {
                File.AppendAllText
                (
                    "Logs/Errors.txt",
                    DateTime.Now +
                    " | " +
                    context.Exception.Message +
                    "\n" +
                    context.Exception.InnerException +
                    "\n\nTrace : \n\n" +
                    context.Exception.StackTrace
                    + new string('-', 100) +
                    "\n\n"
                );

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine
                (
                    DateTime.Now +
                    " | " +
                    context.Exception.Message +
                    "\n" +
                    context.Exception.InnerException +
                    "\n\nTrace : \n\n" +
                    context.Exception.StackTrace
                    + new string('-', 100)
                );
                Console.ResetColor();
            }

            try
            {
                context.Result = new ObjectResult(new
                {
                    Error = JsonSerializer.Deserialize<object>(Error),

                })
                {
                    StatusCode = StatusCode
                };
            }
            catch
            {
                context.Result = new ObjectResult(new
                {
                    Error,

                })
                {
                    StatusCode = StatusCode
                };
            }
            context.ExceptionHandled = true;
        }
    }
}