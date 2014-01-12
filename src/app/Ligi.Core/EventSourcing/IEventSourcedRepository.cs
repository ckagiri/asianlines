using System;

namespace Ligi.Core.EventSourcing
{
    public interface IEventSourcedRepository<T> where T : IEventSourced
    {
        T Find(Guid id);
        T Get(Guid id);
        void Save(T eventSourced, string correlationId);
    }
}
