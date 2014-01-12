using System;
using System.Collections.Generic;
using Ligi.Core.Model;

namespace Ligi.Core.Events.Contracts
{
    public class BetslipSubmitted : VersionedEvent
    {
        public Guid BetslipId { get; set; }
        public string UserName { get; set; }
        public Guid SeasonId { get; set; }
        public Guid BookieId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Bet> Bets { get; set; }
        public SeasonAccount SeasonAccount { get; set; }
    }
}
