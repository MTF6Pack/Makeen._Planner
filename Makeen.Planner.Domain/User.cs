using Domain.Report;
using Domain.Task;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class User : IdentityUser<Guid>
    {
        public int Age { get; private set; }
        public Guid? Avatarid { get; private set; }
        public List<Group>? Groups { get; private set; }
        public List<Chart>? Charts { get; private set; }
        public DateTime CreationTime { get; private set; }
        public List<Task.Task>? Tasks { get; private set; }
        public static void AgeValidation(int age)
        {
            if (age <= 8) throw new Exception("Age must be more than 8");
        }
        public User(string username, string email, int age, string phonenumber, Guid? avatarid)
        {
            Age = age;
            Email = email;
            AgeValidation(age);
            Id = Guid.NewGuid();
            Avatarid = avatarid;
            UserName = username;
            PhoneNumber = phonenumber;
            CreationTime = DateTime.Now;
        }
        public void UpdateUser(string username, string email, int age, string phonenumber, Guid? avatarid)
        {
            Age = age;
            Email = email;
            AgeValidation(age);
            UserName = username;
            Avatarid = avatarid;
            PhoneNumber = phonenumber;
        }
        public User()
        {

        }
    }
}
