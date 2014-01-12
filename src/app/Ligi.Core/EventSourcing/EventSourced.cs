using Ligi.Core.Events;
using System;
using System.Collections.Generic;

namespace Ligi.Core.EventSourcing
{
    public abstract class EventSourced : IEventSourced
    {
        private readonly Dictionary<Type, Action<IVersionedEvent>> _handlers = new Dictionary<Type, Action<IVersionedEvent>>();
        private readonly List<IVersionedEvent> _pendingEvents = new List<IVersionedEvent>();

        private readonly Guid _id;
        private int _version = -1;

        protected EventSourced(Guid id)
        {
            _id = id;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public int Version
        {
            get { return _version; }
            protected set { _version = value; }
        }

        public IEnumerable<IVersionedEvent> Events
        {
            get { return _pendingEvents; }
        }

        protected void Handles<TEvent>(Action<TEvent> handler)
            where TEvent : IEvent
        {
            _handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }

        protected void LoadFrom(IEnumerable<IVersionedEvent> pastEvents)
        {
            foreach (var e in pastEvents)
            {
                _handlers[e.GetType()].Invoke(e);
                _version = e.Version;
            }
        }

        protected void Update(VersionedEvent e)
        {
            e.SourceId = Id;
            e.Version = _version + 1;
            _handlers[e.GetType()].Invoke(e);
            _version = e.Version;
            _pendingEvents.Add(e);
        }
    }
}
