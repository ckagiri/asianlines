using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.AspnetMembership
{
    public class UsersOpenAuthData
    {
        public UsersOpenAuthData()
        {
            this.UsersOpenAuthAccounts = new List<UsersOpenAuthAccount>();
        }

        public string ApplicationName { get; set; }
        public string MembershipUserName { get; set; }
        public bool HasLocalPassword { get; set; }
        public virtual ICollection<UsersOpenAuthAccount> UsersOpenAuthAccounts { get; set; }
    }
}
