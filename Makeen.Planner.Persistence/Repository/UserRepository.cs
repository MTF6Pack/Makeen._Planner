using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repository.Base;
using Persistence.Repository.Interface;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Repository
{
    public class UserRepository(DataBaseContext context) : Repository<User>(context), IUserRepository
    {
    }
}