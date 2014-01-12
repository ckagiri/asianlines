using System;
using System.Linq.Expressions;
using Ligi.Core.DomainBase;

namespace Ligi.Core.DataAccess
{
    public interface IDataContext<T> : IDisposable
        where T : IAggregateRoot
    {
        T Find(Guid id);
        T Find(Expression<Func<T, bool>> predicate);
        void Save(T aggregate);
    }
}
