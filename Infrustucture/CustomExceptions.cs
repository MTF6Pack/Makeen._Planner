using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrustucture
{

    [CustomException(400)]
    public class BadRequestExeption : Exception
    {
        readonly static string defaultmessage = " Error Invalid input";
        public BadRequestExeption() : base(JsonSerializer.Serialize(new { Message = defaultmessage })) { }
        public BadRequestExeption(string message) : base(JsonSerializer.Serialize(new { Message = message + defaultmessage })) { }
        public BadRequestExeption(string message, object content) : base(JsonSerializer.Serialize(new { Message = message + defaultmessage, Content = content })) { }
    }

    [CustomException(404)]
    public class NotFoundException : Exception
    {
        readonly static string defaultmessage = " Not found";
        public NotFoundException(string message) : base(JsonSerializer.Serialize(new { Message = message + defaultmessage })) { }
        public NotFoundException(string message, object content) : base(JsonSerializer.Serialize(new { Message = message + defaultmessage, Content = content })) { }
    }
}
