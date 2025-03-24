using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Application.Notification_Service
{
    public record SenderInfoDto
    {
        public string SenderName { get; set; } = string.Empty;
        public string? SenderUserName { get; set; } // Nullable because groups don't have this
        public string? SenderAvatarUrl { get; set; } = string.Empty;
        public string? SenderColor { get; set; }
    }
}
