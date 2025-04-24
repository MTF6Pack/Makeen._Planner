using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class NotificationCreatedEvent(Guid userId, string message) : INotification
    {
        public Guid UserId { get; } = userId;
        public string Message { get; } = message;
    }
}
