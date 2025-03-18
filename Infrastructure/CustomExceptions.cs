namespace Infrastructure
{

    [CustomException(400)]
    public class BadRequestException : Exception
    {
        readonly static string defaultmessage = " Error Invalid input";
        public BadRequestException() : base(defaultmessage) { }
        public BadRequestException(string message) : base(message) { }
    }

    [CustomException(401)]
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base("Missing or Expired Token") { }
        public UnauthorizedException(string message) : base(message) { }
    }

    [CustomException(404)]
    public class NotFoundException(string message) : Exception(message + defaultmessage)
    {
        readonly static string defaultmessage = " Not found";
    }
}
