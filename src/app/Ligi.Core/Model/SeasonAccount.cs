using System;

namespace Ligi.Core.Model
{
    public class SeasonAccount
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public Guid SeasonId { get; set; }
        public decimal TotalStake { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal Profit { get; set; }
        public int BetsPlaced { get; set; }
        public int BetsSettled { get; set; }
        public DateTime LatestBetTimeStamp { get; set; }
    }
}
