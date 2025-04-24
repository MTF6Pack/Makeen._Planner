namespace Persistence
{
    public record TaskDapperDto
    {
        public string Status { get; set; } = string.Empty;
        public DateTime DeadLine { get; set; }
        public int UpcomingTasksCount { get; set; }
    }

    public record RawNotificationDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public string Result { get; set; } = string.Empty;
        public string? GroupName { get; set; }
        public string? GroupPhoto { get; set; }
        public string? GroupColor { get; set; }
        public string? SenderName { get; set; }
        public string? SenderUserName { get; set; }
        public string? SenderPhoto { get; set; }
    }

    public record NotificationDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public string Result { get; set; } = string.Empty;
        public SenderInfo? SenderInfo { get; set; }
    }

    public record SenderInfo
    {
        public string? SenderColor { get; set; }
        public string? SenderName { get; set; }
        public string? SenderUsername { get; set; }
        public string? SenderPhoto { get; set; }
    }
}
