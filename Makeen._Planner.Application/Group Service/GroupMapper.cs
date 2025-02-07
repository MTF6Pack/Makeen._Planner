using Domain;
using Domain.Task;
using Makeen._Planner.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Group_Service
{
    public static class GroupMapper
    {
        public static Group ToModel(this AddGroupCommand command)
        {
            Group group = new(command.Title, command.AvatarId, command.Color, command.OwnerId);
            return group;
        }
    }
}
