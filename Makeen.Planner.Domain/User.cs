using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class User : IdentityUser<Guid>
    {
        public string? AvatarUrl { get; private set; }
        public List<Group> Groups { get; private set; } = [];
        public DateTime CreationTime { get; private set; }
        public List<Task.Task> Tasks { get; private set; } = [];

        public User(string email)
        {
            Email = email;
            UserName = email[..email.IndexOf('@')];
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
        }
        public void UpdateUser(string username, string email, string? avatarurl, string phonenumber)
        {
            Email = email;
            UserName = username;
            AvatarUrl = avatarurl;
            PhoneNumber = phonenumber;
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
