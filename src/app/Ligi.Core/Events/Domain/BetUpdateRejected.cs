using System;
using Ligi.Core.Model;

namespace Ligi.Core.Events.Contracts
{
    public class BetUpdateRejected : VersionedEvent
    {
        public Guid UserId { get; set; }
        public Bet Bet { get; set; }
    }
}
