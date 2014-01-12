using System;
using Ligi.Core.Model;

namespace Ligi.Core.Events.Contracts
{
    public class MonthAccountUpdated : VersionedEvent
    {
        public MonthAccount MonthAccount { get; set; }
    }
}
