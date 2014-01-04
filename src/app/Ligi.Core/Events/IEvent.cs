using System;

namespace Ligi.Core.Events
{
    public interface IEvent
    {
        Guid SourceId { get; }
    }
}
