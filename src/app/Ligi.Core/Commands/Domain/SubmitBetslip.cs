using System;
using System.Collections.Generic;
using Ligi.Core.Model;

namespace Ligi.Core.Commands.Contracts
{
    public class SubmitBetslip : ICommand
    {
        public SubmitBetslip()
        {
            Id = Guid.NewGuid();
            Bets = new List<BetItem>();
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid SeasonId { get; set; }
        public List<BetItem> Bets { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
