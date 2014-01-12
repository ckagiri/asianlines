using System;
using Ligi.Core.Model;

namespace Ligi.Core.Events.Contracts
{
    public class MatchResultConfirmed : IEvent
    {
        public Guid SourceId { get; set; }
        public Guid FixtureId { get; set; }
        public MatchStatus MatchStatus { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}
