using Ligi.Core.EventSourcing;
using Ligi.Core.Events;
using Ligi.Core.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ligi.Infrastructure.Sql.Common;
using Ligi.Infrastructure.Sql.Serialization;

namespace Ligi.Infrastructure.Sql.EventSourcing
{
    public class SqlEventSourcedRepository<T> : IEventSourcedRepository<T> where T : class, IEventSourced
    {
        // Could potentially use DataAnnotations to get a friendly/unique name in case of collisions between BCs.
        private static readonly string sourceType = typeof(T).Name;
        private readonly IEventBus _eventBus;
        private readonly ITextSerializer _serializer;
        private readonly Func<EventStoreDbContext> _contextFactory;
        private readonly Func<Guid, IEnumerable<IVersionedEvent>, T> _entityFactory;

        public SqlEventSourcedRepository(IEventBus eventBus, ITextSerializer serializer, Func<EventStoreDbContext> contextFactory)
        {
            _eventBus = eventBus;
            _serializer = serializer;
            _contextFactory = contextFactory;

            // TODO: could be replaced with a compiled lambda
            var constructor = typeof(T).GetConstructor(new[] { typeof(Guid), typeof(IEnumerable<IVersionedEvent>) });
            if (constructor == null)
            {
                throw new InvalidCastException("Type T must have a constructor with the following signature: .ctor(Guid, IEnumerable<IVersionedEvent>)");
            }
            _entityFactory = (id, events) => (T)constructor.Invoke(new object[] { id, events });
        }

        public T Find(Guid id)
        {
            using (var context = _contextFactory.Invoke())
            {
                var deserialized = context.Set<Event>()
                    .Where(x => x.AggregateId == id && x.AggregateType == sourceType)
                    .OrderBy(x => x.Version)
                    .AsEnumerable()
                    .Select(Deserialize)
                    .AsCachedAnyEnumerable();

                if (deserialized.Any())
                {
                    return _entityFactory.Invoke(id, deserialized);
                }

                return null;
            }
        }

        public T Get(Guid id)
        {
            var entity = Find(id);
            if (entity == null)
            {
                throw new EntityNotFoundException(id, sourceType);
            }

            return entity;
        }

        public void Save(T eventSourced, string correlationId)
        {
            // TODO: guarantee that only incremental versions of the event are stored
            var events = eventSourced.Events.ToArray();
            using (var context = _contextFactory.Invoke())
            {
                var eventsSet = context.Set<Event>();
                foreach (var e in events)
                {
                    eventsSet.Add(Serialize(e, correlationId));
                }

                context.SaveChanges();
            }

            // TODO: guarantee delivery or roll back, or have a way to resume after a system crash
            _eventBus.Publish(events.Select(e => new Envelope<IEvent>(e) { CorrelationId = correlationId }));
        }

        private Event Serialize(IVersionedEvent e, string correlationId)
        {
            Event serialized;
            using (var writer = new StringWriter())
            {
                _serializer.Serialize(writer, e);
                serialized = new Event
                {
                    AggregateId = e.SourceId,
                    AggregateType = sourceType,
                    Version = e.Version,
                    Payload = writer.ToString(),
                    CorrelationId = correlationId
                };
            }
            return serialized;
        }

        private IVersionedEvent Deserialize(Event @event)
        {
            using (var reader = new StringReader(@event.Payload))
            {
                return (IVersionedEvent)_serializer.Deserialize(reader);
            }
        }
    }
}