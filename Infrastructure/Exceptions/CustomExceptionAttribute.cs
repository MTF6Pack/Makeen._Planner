using System;

namespace Infrastructure.Exceptions
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CustomExceptionAttribute(int statusCode) : Attribute
    {
        public int StatusCode { get; } = statusCode;
    }
}