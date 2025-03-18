namespace Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CustomExceptionAttribute(int statusCode) : Attribute
    {
        public int StatusCode { get; set; } = statusCode;
    }
}