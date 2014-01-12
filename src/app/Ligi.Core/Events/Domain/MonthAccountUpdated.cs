using Ligi.Core.Model;

namespace Ligi.Core.Events.Domain
{
    public class MonthAccountUpdated : VersionedEvent
    {
        public MonthAccount MonthAccount { get; set; }
    }
}
