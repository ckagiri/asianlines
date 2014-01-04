using System.Collections.Generic;
using Ligi.Core.Events;

namespace Ligi.Core.Messaging
{
    public interface IEventBus
    {
        void Publish(Envelope<IEvent> @event);
        void Publish(IEnumerable<Envelope<IEvent>> events);
    }
}
