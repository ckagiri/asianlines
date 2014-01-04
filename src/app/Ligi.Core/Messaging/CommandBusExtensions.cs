using System.Collections.Generic;
using System.Linq;
using Ligi.Core.Commands;

namespace Ligi.Core.Messaging
{
    public static class CommandBusExtensions
    {
        public static void Send(this ICommandBus bus, ICommand command)
        {
            bus.Send(new Envelope<ICommand>(command));
        }

        public static void Send(this ICommandBus bus, IEnumerable<ICommand> commands)
        {
            bus.Send(commands.Select(x => new Envelope<ICommand>(x)));
        }
    }
}
