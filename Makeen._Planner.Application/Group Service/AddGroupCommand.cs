namespace Application.Group_Service
{
    public class AddGroupCommand
    {
        public Guid OwnerId { get; set; }
        public string? AvatarUrl { get; set; }
        public required string Title { get; set; }
        public required string Color { get; set; }
    }

}
