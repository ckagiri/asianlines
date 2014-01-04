using System;

namespace AsianLines.Core.Events
{
    public interface IEvent
    {
        Guid SourceId { get; }
    }
}
