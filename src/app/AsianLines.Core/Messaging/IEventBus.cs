using System.Collections.Generic;
using AsianLines.Core.Events;

namespace AsianLines.Core.Messaging
{
    public interface IEventBus
    {
        void Publish(Envelope<IEvent> @event);
        void Publish(IEnumerable<Envelope<IEvent>> events);
    }
}
