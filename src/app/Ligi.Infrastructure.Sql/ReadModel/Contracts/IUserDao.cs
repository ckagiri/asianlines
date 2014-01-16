using System;

namespace Ligi.Infrastructure.Sql.ReadModel.Contracts
{
    public interface IUserDao
    {
        string GetUserName(Guid userId);
    }
}
