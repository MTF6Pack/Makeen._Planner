using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrustucture
{

    [CustomException(400)]
    public class BadRequestException : Exception
    {
        readonly static string defaultmessage = " Error Invalid input";
        public BadRequestException() : base(JsonSerializer.Serialize(new { Message = defaultmessage })) { }
        public BadRequestException(string message) : base(JsonSerializer.Serialize(new { Message = message + defaultmessage })) { }
        public BadRequestException(string message, object content) : base(JsonSerializer.Serialize(new { Message = message + defaultmessage, Content = content })) { }
    }

    [CustomException(404)]
    public class NotFoundException : Exception
    {
        readonly static string defaultmessage = " Not found";
        public NotFoundException(string message) : base(JsonSerializer.Serialize(new { Message = message + defaultmessage })) { }
        public NotFoundException(string message, object content) : base(JsonSerializer.Serialize(new { Message = message + defaultmessage, Content = content })) { }
    }
}
