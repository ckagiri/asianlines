using System;

namespace Ligi.Infrastructure.Sql.ReadModel
{
    public class SeasonLeaderBoard
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid SeasonId { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal TotalStake { get; set; }
        public decimal Profit { get; set; }
        public DateTime LatestBetTimeStamp { get; set; }
        public int Version { get; set; }
        public int BetsPlaced { get; set; }
        public int BetsSettled { get; set; }
    }

    public class MonthLeaderBoard
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid SeasonId { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal TotalStake { get; set; }
        public decimal Profit { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime LatestBetTimeStamp { get; set; }
        public int Version { get; set; }
        public int BetsPlaced { get; set; }
        public int BetsSettled { get; set; }
    }
}
