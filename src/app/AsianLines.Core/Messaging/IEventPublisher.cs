using System.Collections.Generic;
using AsianLines.Core.Events;

namespace AsianLines.Core.Messaging
{
    public interface IEventPublisher
    {
        IEnumerable<IEvent> Events { get; }
    }
}
