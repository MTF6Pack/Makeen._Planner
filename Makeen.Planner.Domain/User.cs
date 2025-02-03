using Domain.Report;
using Domain.Task;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Domain
{
    public class User : IdentityUser<Guid>
    {
        public int Age { get; private set; }
        public DateTime CreationTime { get; private set; }
        [NotMapped]
        public bool HasAvatar { get; private set; }
        public List<Task.Task>? Tasks { get; private set; }
        public List<Group>? Groups { get; private set; }
        public List<Chart>? Charts { get; private set; }
        public User(string username, string email, int age, string phonenumber)
        {
            UserName = username;
            AgeValidation(age);
            Age = age;
            Email = email;
            PhoneNumber = phonenumber;
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
            HasAvatar = false;
        }
        public static void AgeValidation(int age)
        {
            if (age <= 8) throw new Exception("Age must be more than 8");
        }
        public void UpdateUser(string username, string email, int age, string phonenumber, IFormFile avatar, Guid Id)
        {
            UserName = username;
            AgeValidation(age);
            Age = age;
            Email = email;
            PhoneNumber = phonenumber;
            var AvatarData = new MemoryStream();
            if (avatar != null)
            {
                File.WriteAllBytes("C:\\Users\\MTF\\source\\repos\\Main project Makeen_Planner\\Makeen._Planner\\" + Id + ".png", AvatarData.ToArray());
                avatar.CopyTo(AvatarData);
            }
        }
        public User()
        {

        }
    }
}
