using System;
using Ligi.Core.Model;

namespace Ligi.Core.Commands.Contracts
{
    public class TransactPayout : ICommand
    {
        public TransactPayout()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public Guid BookieId { get; set; }
        public Guid FixtureId { get; set; }
        public MatchStatus MatchStatus { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}
