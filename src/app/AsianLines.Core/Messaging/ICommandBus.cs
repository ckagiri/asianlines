using System.Collections.Generic;
using AsianLines.Core.Commands;

namespace AsianLines.Core.Messaging
{
    public interface ICommandBus
    {
        void Send(Envelope<ICommand> command);
        void Send(IEnumerable<Envelope<ICommand>> commands);
    }
}
