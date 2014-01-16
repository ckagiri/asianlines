using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ligi.Core.Commands;
using Ligi.Infrastructure.Sql.Messaging;
using Ligi.Infrastructure.Sql.Messaging.Handling;
using Ligi.Infrastructure.Sql.Serialization;

namespace Ligi.Infrastructure.Messaging.Handling
{
    public class CommandProcessor : MessageProcessor, ICommandHandlerRegistry
    {
        private readonly Dictionary<Type, ICommandHandler> _handlers = new Dictionary<Type, ICommandHandler>();

        public CommandProcessor(IMessageReceiver receiver, ITextSerializer serializer)
            : base(receiver, serializer)
        {
        }

        public void Register(ICommandHandler commandHandler)
        {
            var genericHandler = typeof(ICommandHandler<>);
            var supportedCommandTypes = commandHandler.GetType()
                .GetInterfaces()
                .Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == genericHandler)
                .Select(iface => iface.GetGenericArguments()[0])
                .ToList();

            if (_handlers.Keys.Any(registeredType => supportedCommandTypes.Contains(registeredType)))
                throw new ArgumentException("The command handled by the received handler already has a registered handler.");

            // Register this handler for each of the handled types.
            foreach (var commandType in supportedCommandTypes)
            {
                _handlers.Add(commandType, commandHandler);
            }
        }

        protected override void ProcessMessage(object payload, string correlationId)
        {
            var commandType = payload.GetType();
            ICommandHandler handler = null;

            if (_handlers.TryGetValue(commandType, out handler))
            {
                Trace.WriteLine("-- Handled by " + handler.GetType().FullName);
                ((dynamic)handler).Handle((dynamic)payload);
            }

            // There can be a generic logging/tracing/auditing handlers
            if (_handlers.TryGetValue(typeof(ICommand), out handler))
            {
                Trace.WriteLine("-- Handled by " + handler.GetType().FullName);
                ((dynamic)handler).Handle((dynamic)payload);
            }
        }
    }
}
