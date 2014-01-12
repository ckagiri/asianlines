using System;
using System.Collections.Generic;
using Ligi.Core.Model;

namespace Ligi.Core.Commands.Contracts
{
    public class UpdateSeasonAccount : ICommand
    {
        public UpdateSeasonAccount()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public Guid SeasonId { get; set; }
        public Guid BettorId { get; set; }
        public Guid BookieId { get; set; }
        public List<BetTransaction> BetTransactions { get; set; }
    }
}
