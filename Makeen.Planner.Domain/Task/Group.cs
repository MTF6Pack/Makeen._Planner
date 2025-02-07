using Domain.Report;

namespace Domain.Task
{
    public class Group
    {

        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Color { get; private set; } = string.Empty;
        public Guid OwnerId { get; private set; }
        public Guid? AvatarId { get; private set; }
        public List<User>? Members { get; private set; }
        public List<Task>? Tasks { get; private set; }
        public List<Chart>? Charts { get; private set; }

        public Group(string title, Guid? avatarid, string color, Guid ownerId)
        {
            Title = title;
            Color = color;
            OwnerId = ownerId;
            AvatarId = avatarid;
            Id = Guid.NewGuid();
        }

        public void UpdateGroup(string title, Guid avatarurl, string color)
        {
            Title = title;
            AvatarId = avatarurl;
            Color = color;
        }

        public Group()
        {
        }
    }
}
