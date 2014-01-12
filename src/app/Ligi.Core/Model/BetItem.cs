using System;

namespace Ligi.Core.Model
{
    public class BetItem : Bet
    {
        public DateTime StartOfWeek { get; set; }
        public DateTime EndOfWeek { get; set; }
        public DateTime FixtureKickOff { get; set; }
    }
}
