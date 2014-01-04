using System;

namespace AsianLines.Core.DomainBase
{
    public interface IAggregateRoot
    {
        Guid Id { get; }
    }
}
