using System;
using Ligi.Core.Model;

namespace Ligi.Core.Events.Contracts
{
    public class FixtureAccountUpdated : VersionedEvent
    {
        public Guid SeasonId { get; set; }
        public FixtureSubAccount FixtureSubAccount { get; set; }
    }
}
