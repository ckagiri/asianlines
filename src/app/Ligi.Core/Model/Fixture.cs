using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Ligi.Core.Model
{
    public class Fixture
    {
        public Fixture()
        {
            HomeAsianHandicap = -1;
            AwayAsianHandicap = -1;
            Odds = new Collection<MatchOdds>();
        }
        public Guid Id { get; set; }
        public Guid SeasonId { get; set; }
        public Guid HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }
        public Guid AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }
        public DateTime KickOff { get; set; }
        public string Venue { get; set; }
        public MatchStatus MatchStatus { get; set; }
        [Range(0, 15)]
        public int HomeScore { get; set; }
        [Range(0, 15)]
        public int AwayScore { get; set; }
        public decimal HomeAsianHandicap { get; set; }
        public decimal AwayAsianHandicap { get; set; }
        public decimal AsianGoalsHandicap { get; set; }
        public DateTime StartOfWeek { get; set; }
        public DateTime EndOfWeek { get; set; }
        public ICollection<MatchOdds> Odds { get; set; }
        public bool MatchResultConfirmed { get; set; }
    }

    public enum MatchStatus
    {
        Pending,
        InProgress,
        Played,
        Cancelled,
        Abandoned,
        PostPoned
    }
}
