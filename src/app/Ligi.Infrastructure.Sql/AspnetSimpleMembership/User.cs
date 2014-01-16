using System;
using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.AspnetSimpleMembership
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        public ICollection<Role> Roles { get; set; }

        public User() {
            this.Roles = new List<Role>();
        }
    }
}
