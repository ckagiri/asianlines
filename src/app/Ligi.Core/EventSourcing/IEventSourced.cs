using Ligi.Core.Events;
using System;
using System.Collections.Generic;

namespace Ligi.Core.EventSourcing
{
    public interface IEventSourced
    {
        Guid Id { get; }
        int Version { get; }
        IEnumerable<IVersionedEvent> Events { get; }
    }
}
