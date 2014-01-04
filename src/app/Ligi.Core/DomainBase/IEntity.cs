using System;

namespace Ligi.Core.DomainBase
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
