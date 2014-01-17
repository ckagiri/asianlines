using System;
using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.AspnetMembership
{
    public class Role
    {
        public Role()
        {
            this.Users = new List<User>();
        }

        public Guid RoleId { get; set; }
        public Guid ApplicationId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public virtual Application Application { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
