using System;
using Ligi.Core.Model;

namespace Ligi.Core.Events.Domain
{
    public class BetUpdated : VersionedEvent
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Bet Bet { get; set; }
    }
}
