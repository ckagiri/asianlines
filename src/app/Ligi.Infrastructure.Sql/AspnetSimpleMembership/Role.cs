using System.Collections.Generic;
using Ligi.Core.Model;

namespace Ligi.Infrastructure.Sql.AspnetSimpleMembership
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public ICollection<User> Users { get; set; }

        public Role()
        {
            this.Users = new List<User>();
        }
    }
}
