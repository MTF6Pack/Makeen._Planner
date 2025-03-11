using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class User : IdentityUser<Guid>
    {
        public string? AvatarUrl { get; private set; }
        public string? Fullname { get; private set; }
        public List<GroupMembership> Memberships { get; private set; } = [];
        public DateTime CreationTime { get; private set; }
        public List<Task.Task> Tasks { get; private set; } = [];

        public User(string email)
        {
            Email = email;
            UserName = email[..email.IndexOf('@')];
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
        }
        public void UpdateUser(string username, string email, string? avatarurl, string phonenumber, string? fullname)
        {
            Email = email;
            UserName = username;
            AvatarUrl = avatarurl;
            PhoneNumber = phonenumber;
            Fullname = fullname;
        }

        public void UpdateUserAvatar(string avatarurl)
        {
            AvatarUrl = avatarurl;
        }

        public User()
        {

        }
    }
}
