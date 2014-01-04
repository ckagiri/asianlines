using System;

namespace AsianLines.Core.Commands
{
    public interface ICommand
    {
        Guid Id { get; }
    }
}
