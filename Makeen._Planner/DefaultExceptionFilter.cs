//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace Mediat_RRR
//{
//    public class DefaultExceptionFilter : IExceptionFilter
//    {
//        public void OnException(ExceptionContext context)
//        {
//            var ErrorType = context.Exception.GetType();
//            var Error = "An Error Occured";
//            var StatusCode = 400;

//            if (ErrorType == typeof(UnauthorizedExceptio))
//            {
//                Error = context.Exception.Message;
//                StatusCode = 401;
//            }

//            context.Result = new ObjectResult(new { Error })
//            { StatusCode = StatusCode };
//        }
//    }
//}
