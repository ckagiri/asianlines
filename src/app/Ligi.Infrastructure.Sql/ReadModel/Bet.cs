using System;
using Ligi.Core.Model;

namespace Ligi.Infrastructure.Sql.ReadModel
{
    public class Bet
    {
        public Guid Id { get; set; }
        public Guid SeasonId { get; set; }
        public Guid FixtureId { get; set; }
        public BetType BetType { get; set; }
        public decimal Handicap { get; set; }
        public BetPick BetPick { get; set; }
        public decimal Stake { get; set; }
        public decimal Payout { get; set; }
        public decimal Profit { get; set; }
        public Guid UserId { get; set; }
        public Guid BookieId { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Version { get; set; }
        public BetResult BetResult { get; set; }
    }
}
