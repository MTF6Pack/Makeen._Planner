using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class TaskReminderEvent(Guid userId, Notification notification) : INotification
    {
        public Guid UserId { get; } = userId;
        public Notification Notification { get; } = notification;
    }
}
