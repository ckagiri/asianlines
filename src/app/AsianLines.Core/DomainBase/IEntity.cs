using System;

namespace AsianLines.Core.DomainBase
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
