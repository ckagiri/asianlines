using System;

namespace Ligi.Core.Commands
{
    public interface ICommand
    {
        Guid Id { get; }
    }
}
