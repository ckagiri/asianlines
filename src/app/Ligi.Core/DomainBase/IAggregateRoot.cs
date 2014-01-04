using System;

namespace Ligi.Core.DomainBase
{
    public interface IAggregateRoot
    {
        Guid Id { get; }
    }
}
