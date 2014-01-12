using System;
using System.Collections.Generic;
using Ligi.Core.Model;

namespace Ligi.Core.Events.Contracts
{
    public class BetsProcessed : VersionedEvent
    {
        public Guid BetslipId { get; set; }
        public List<BetTransaction> BetTransactions { get; set; }
    }
}
