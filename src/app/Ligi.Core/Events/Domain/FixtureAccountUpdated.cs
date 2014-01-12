using System;
using Ligi.Core.Model;

namespace Ligi.Core.Events.Domain
{
    public class FixtureAccountUpdated : VersionedEvent
    {
        public Guid SeasonId { get; set; }
        public FixtureSubAccount FixtureSubAccount { get; set; }
    }
}
