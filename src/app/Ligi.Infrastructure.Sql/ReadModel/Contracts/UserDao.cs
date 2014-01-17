using System;
using System.Linq;
using Ligi.Infrastructure.Sql.AspnetMembership;
using Ligi.Infrastructure.Sql.Database;

namespace Ligi.Infrastructure.Sql.ReadModel.Contracts
{
    public class UserDao : IUserDao
    {
        private readonly Func<AdminDbContext> _contextFactory;

        public UserDao(Func<AdminDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public string GetUserName(Guid userId)
        {
            using (var context = _contextFactory.Invoke())
            {
                var user = context.Query<User>().FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    return user.UserName;
                }
                return " ";
            }
        }
    }
}
