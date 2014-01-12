using System;
using Ligi.Core.Model;

namespace Ligi.Core.Events.Domain
{
    public class PayoutTransacted : VersionedEvent
    {
        public Guid UserId { get; set; }
        public Bet Bet { get; set; }
        public BetResult BetResult { get; set; }
    }
}
