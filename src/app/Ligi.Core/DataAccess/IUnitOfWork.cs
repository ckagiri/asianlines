using System;

namespace Ligi.Core.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        int Commit();
        IContext Context { get; }
    }
}
