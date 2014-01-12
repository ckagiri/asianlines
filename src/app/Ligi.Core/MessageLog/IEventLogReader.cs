using Ligi.Core.Events;
using System.Collections.Generic;

namespace Ligi.Core.MessageLog
{
    /// <summary>
    /// Exposes the message log for all events that the system processed.
    /// </summary>
    public interface IEventLogReader
    {
        IEnumerable<IEvent> Query(QueryCriteria criteria);
    }
}
