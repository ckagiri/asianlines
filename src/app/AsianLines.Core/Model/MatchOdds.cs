using System;

namespace AsianLines.Core.Model
{
    public class MatchOdds
    {
        public MatchOdds()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; private set; }
        public Guid FixtureId { get; set; }
        public OddsType OddsType { get; set; }
        public decimal Handicap { get; set; }
    }

    public enum OddsType
    {
        None,
        AsianHandicap,
        AsianGoals
    }
}
