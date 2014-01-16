using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ligi.Core.Events;
using Ligi.Core.Messaging;
using Ligi.Infrastructure.Messaging;
using Ligi.Infrastructure.Sql.Serialization;

namespace Ligi.Infrastructure.Sql.Messaging
{
    public class EventBus : IEventBus
    {
        private readonly IMessageSender _sender;
        private readonly ITextSerializer _serializer;

        public EventBus(IMessageSender sender, ITextSerializer serializer)
        {
            _sender = sender;
            _serializer = serializer;
        }

        public void Publish(Envelope<IEvent> @event)
        {
            var message = BuildMessage(@event);

            _sender.Send(message);
        }

        public void Publish(IEnumerable<Envelope<IEvent>> events)
        {
            var messages = events.Select(e => BuildMessage(e));

            _sender.Send(messages);
        }

        private Message BuildMessage(Envelope<IEvent> @event)
        {
            using (var payloadWriter = new StringWriter())
            {
                _serializer.Serialize(payloadWriter, @event.Body);
                return new Message(payloadWriter.ToString(), correlationId: @event.CorrelationId);
            }
        }
    }
}
