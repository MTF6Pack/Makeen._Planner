using Microsoft.AspNetCore.Identity;

namespace Domains
{
    public class User : IdentityUser<Guid>
    {
        public int Age { get; set; }
        public DateTime CreationTime { get; set; }
        public List<Duty>? Duties { get; set; }

        public User(string username, string email, int age, string phonenumber)
        {
            UserName = username;
            AgeValidation(age);
            Age = age;
            Email = email;
            PhoneNumber = phonenumber;
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
        }

        public static void AgeValidation(int age)
        {
            if (age <= 6) throw new Exception("Fuck your age asshole");
        }
        public void UpdateUser(string username, string email, int age, string phonenumber)
        {
            UserName = username;
            AgeValidation(age);
            Age = age;
            Email = email;
            PhoneNumber = phonenumber;
            Id = Guid.NewGuid();
        }
        private User()
        {

        }
    }
}
