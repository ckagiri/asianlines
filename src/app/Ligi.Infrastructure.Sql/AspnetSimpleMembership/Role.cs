using System;
using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.AspnetSimpleMembership
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }

        public ICollection<User> Users { get; set; }

        public Role()
        {
            Users = new List<User>();
        }
    }
}
