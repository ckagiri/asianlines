using System;

namespace Ligi.Core.DataAccess
{
    public interface IContext : IDisposable
    {
        int SaveChanges();
    }
}
