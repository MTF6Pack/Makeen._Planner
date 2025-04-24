using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class QueuedNotification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public string Status { get; set; } = string.Empty;  // 'Pending' or 'Delivered'
        public int RetryCount { get; set; }
        public DateTime LastRetryTime { get; set; }
    }
}
