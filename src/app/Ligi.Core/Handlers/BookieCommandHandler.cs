using Ligi.Core.Commands;
using Ligi.Core.Commands.Domain;
using Ligi.Core.EventSourcing;
using Ligi.Core.Model;
using Ligi.Core.Services;

namespace Ligi.Core.Handlers
{
    public class BookieCommandHandler : 
        ICommandHandler<ProcessBets>,
        ICommandHandler<TransactPayout>
    {
        private readonly IEventSourcedRepository<Bookie> _repository;
        private readonly IPayoutService _payoutService;

        public BookieCommandHandler(IEventSourcedRepository<Bookie> repository, IPayoutService payoutService)
        {
            _repository = repository;
            _payoutService = payoutService;
        }

        public void Handle(ProcessBets command)
        {
            var bookie = _repository.Find(command.BookieId);
            if (bookie == null)
            {
                bookie = new Bookie(command.BookieId, command.StartDate, command.EndDate, command.UserId, command.SeasonId);
            }
            bookie.ProcessBets(command.BetslipId, command.UserName, command.Bets);
            _repository.Save(bookie, command.Id.ToString());
        }

        public void Handle(TransactPayout command)
        {
            var bookie = _repository.Get(command.BookieId);
            bookie.TransactPayout(command.FixtureId, command.MatchStatus, command.HomeScore, command.AwayScore, _payoutService);
            _repository.Save(bookie, command.Id.ToString());
        }
    }
}
