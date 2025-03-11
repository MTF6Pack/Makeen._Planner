using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain
{
    public class GroupMembership(Guid userId, Guid groupId, bool isAdmin = false)
    {
        public Guid UserId { get; private set; } = userId;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public User User { get; private set; } = null!;
        public Guid GroupId { get; private set; } = groupId;
        public Group Group { get; private set; } = null!;
        public bool IsAdmin { get; private set; } = isAdmin;

        public void ToggleAdmin()
        {
            IsAdmin = !IsAdmin;
        }
    }
}