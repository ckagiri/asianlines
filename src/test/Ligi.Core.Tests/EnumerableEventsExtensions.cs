using System.Linq;
using Ligi.Core.EventSourcing;
using Ligi.Core.Events;

namespace Ligi.Core.Tests
{
    public static class EnumerableEventsExtensions
    {
        public static TEvent SingleEvent<TEvent>(this IEventSourced aggregate)
            where TEvent : IEvent
        {
            return (TEvent)aggregate.Events.Single();
        }
    }
}
