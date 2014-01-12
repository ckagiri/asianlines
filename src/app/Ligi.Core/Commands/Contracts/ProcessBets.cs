using System;
using System.Collections.Generic;
using Ligi.Core.Model;

namespace Ligi.Core.Commands.Contracts
{
    public class ProcessBets : ICommand
    {
        public ProcessBets()
        {
            Id = Guid.NewGuid();
            Bets = new List<Bet>();
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid BookieId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Bet> Bets { get; set; }
        public Guid SeasonId { get; set; }
        public Guid BetslipId { get; set; }
    }
}
