using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ligi.Core.Commands;
using Ligi.Core.Messaging;
using Ligi.Infrastructure.Messaging;
using Ligi.Infrastructure.Sql.Serialization;

namespace Ligi.Infrastructure.Sql.Messaging
{
    public class CommandBus : ICommandBus
    {
        private readonly IMessageSender _sender;
        private readonly ITextSerializer _serializer;

        public CommandBus(IMessageSender sender, ITextSerializer serializer)
        {
            _sender = sender;
            _serializer = serializer;
        }

        public void Send(Envelope<ICommand> command)
        {
            var message = BuildMessage(command);

            _sender.Send(message);
        }

        public void Send(IEnumerable<Envelope<ICommand>> commands)
        {
            var messages = commands.Select(command => BuildMessage(command));

            _sender.Send(messages);
        }

        private Message BuildMessage(Envelope<ICommand> command)
        {
            // TODO: should use the Command ID as a unique constraint when storing it.
            using (var payloadWriter = new StringWriter())
            {
                _serializer.Serialize(payloadWriter, command.Body);
                return new Message(payloadWriter.ToString(), command.Delay != TimeSpan.Zero ? (DateTime?)DateTime.UtcNow.Add(command.Delay) : null, command.CorrelationId);
            }
        }
    }
}
