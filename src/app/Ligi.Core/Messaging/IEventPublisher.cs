using System.Collections.Generic;
using Ligi.Core.Events;

namespace Ligi.Core.Messaging
{
    public interface IEventPublisher
    {
        IEnumerable<IEvent> Events { get; }
    }
}
