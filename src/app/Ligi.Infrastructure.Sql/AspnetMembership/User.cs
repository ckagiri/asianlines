using System;
using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.AspnetMembership
{
    public partial class User
    {
        public User()
        {
            this.Roles = new List<Role>();
        }

        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public string UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }
        public virtual Application Application { get; set; }
        public virtual Membership Membership { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
