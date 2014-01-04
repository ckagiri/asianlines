using System.Collections.Generic;
using Ligi.Core.Commands;

namespace Ligi.Core.Messaging
{
    public interface ICommandBus
    {
        void Send(Envelope<ICommand> command);
        void Send(IEnumerable<Envelope<ICommand>> commands);
    }
}
