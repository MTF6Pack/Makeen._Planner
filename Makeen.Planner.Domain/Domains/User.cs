using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace Makeen.Planner.Domain.Domains
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; private set; }
        //public string Email { get; private set; }
        public string Password { get; private set; }
        public string RepeatPassword { get; private set; }
        //public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime CreationTime { get; private set; } = DateTime.Now;
        public List<Duty>? Duties { get; private set; }

        public User(string name, string email, string password, string repeatPassword) : base(email)
        {
            Name = name;
            Email = email;
            Password = password;
            RepeatPassword = repeatPassword;
        }

        public static void AgeValidation(int age)
        {
            if (age <= 6) throw new Exception("Fuck your age asshole");
        }
        public void UpdateUser(string name)
        {
            if (name != null) Name = name;
        }
    }
}
