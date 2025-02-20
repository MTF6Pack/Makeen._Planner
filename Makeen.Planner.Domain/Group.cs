using System.Text.Json.Serialization;

namespace Domain
{
    public class Group
    {

        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Color { get; private set; } = string.Empty;
        public Guid OwnerId { get; private set; }
        public string? AvatarUrl { get; private set; }
        [JsonIgnore]
        public List<User>? Members { get; private set; }
        public List<Task.Task>? Tasks { get; private set; }

        public Group(string title, string? avatar, string color, Guid ownerId)
        {
            Title = title;
            Color = color;
            OwnerId = ownerId;
            AvatarUrl = avatar;
            Id = Guid.NewGuid();
        }

        public void UpdateGroup(string title, string? avatar, string color)
        {
            Title = title;
            AvatarUrl = avatar;
            Color = color;
        }

        public Group()
        {
        }
    }
}
