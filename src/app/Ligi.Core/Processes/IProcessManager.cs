using Ligi.Core.Commands;
using Ligi.Core.Messaging;
using System;
using System.Collections.Generic;

namespace Ligi.Core.Processes
{
    public interface IProcessManager
    {
        Guid Id { get; }
        bool Completed { get; }
        IEnumerable<Envelope<ICommand>> Commands { get; }
    }
}
