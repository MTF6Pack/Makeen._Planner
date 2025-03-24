using System.Text.Json.Serialization;

namespace Domain
{
    public class Group
    {

        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Color { get; private set; } = string.Empty;
        public Guid OwnerId { get; private set; }
        public bool Grouptype { get; private set; }
        public string? AvatarUrl { get; private set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GroupMembership> GroupMemberships { get; private set; } = [];
        public List<Domain.Task>? Tasks { get; private set; }

        public Group(string title, string? avatar, string color, Guid ownerId, bool grouptype = false)
        {
            Title = title;
            Color = color;
            OwnerId = ownerId;
            AvatarUrl = avatar;
            Grouptype = grouptype;
            Id = Guid.NewGuid();
        }

        public void AddMember(Guid userId, bool isAdmin = false)
        {
            GroupMemberships.Add(new GroupMembership(userId, this.Id, isAdmin));
        }

        public void UpdateTitle(string title)
        {
            Title = title;
        }

        public void UpdateColor(string color)
        {
            Color = color;
        }

        public void UpdateAvatar(string? avatarUrl)
        {
            AvatarUrl = avatarUrl;
        }

        public Group()
        {
        }
    }
}