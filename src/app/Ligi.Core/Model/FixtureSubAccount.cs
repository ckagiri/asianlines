using System;

namespace Ligi.Core.Model
{
    public class FixtureSubAccount
    {
        public Guid Id { get; set; }
        public Guid FixtureId { get; set; }
        public BetType BetType { get; set; }
        public BetPick BetPick { get; set; }
        public decimal TotalStake { get; set; }
        public decimal TotalPayout { get; set; }
        public int BetsPlaced { get; set; }
        public int BetsSettled { get; set; }
        public DateTime LatestBetTimeStamp { get; set; }
    }
}
