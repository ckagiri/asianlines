using System;

namespace AsianLines.Core.DataAccess
{
    public interface IContext : IDisposable
    {
        int SaveChanges();
    }
}
