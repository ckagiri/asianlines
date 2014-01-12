using System;

namespace Ligi.Core.Model
{
    public class Bet
    {
        public Guid Id { get; set; }
        public Guid FixtureId { get; set; }
        public BetType BetType { get; set; }
        public decimal Handicap { get; set; }
        public BetPick BetPick { get; set; }
        public decimal Wager { get; set; }
        public decimal Stake { get; set; }
        public decimal Payout { get; set; }
        public DateTime TimeStamp { get; set; }
        public decimal Profit { get; set; }
        public DateTime MatchDay { get; set; }
    }

    public enum BetType
    {
        None,
        AsianHandicap,
        AsianGoals
    }

    public enum BetPick
    {
        None,
        Home,
        Away,
        Over,
        Under
    }

    public enum BetResult
    {
        Void,
        Win,
        HalfWin,
        Push,
        Lose,
        HalfLose
    }
}
