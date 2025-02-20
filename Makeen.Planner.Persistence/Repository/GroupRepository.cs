using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.Repository.Base;
using Persistence.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repository
{
    public class GroupRepository(DataBaseContext context) : Repository<Group>(context), IGroupRepository
    {
    }
}
