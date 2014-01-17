using System;
using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.AspnetMembership
{
    public class Application
    {
        public Application()
        {
            this.Memberships = new List<Membership>();
            this.Roles = new List<Role>();
            this.Users = new List<User>();
        }

        public Guid ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
