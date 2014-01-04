using System;

namespace AsianLines.Core.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        int Commit();
        IContext Context { get; }
    }
}
