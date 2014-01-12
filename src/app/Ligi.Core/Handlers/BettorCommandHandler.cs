using Ligi.Core.Commands;
using Ligi.Core.Commands.Domain;
using Ligi.Core.EventSourcing;
using Ligi.Core.Model;

namespace Ligi.Core.Handlers
{
    public class BettorCommandHandler :
        ICommandHandler<SubmitBetslip>,
        ICommandHandler<UpdateSeasonAccount>
    {
        private readonly IEventSourcedRepository<Bettor> _repository;

        public BettorCommandHandler(IEventSourcedRepository<Bettor> repository)
        {
            _repository = repository;
        }

        public void Handle(SubmitBetslip command)
        {
            var bettor = _repository.Find(command.UserId);
            if (bettor == null)
            {
                bettor = new Bettor(command.UserId);
            }

            bettor.SubmitBetslip(command.UserName, command.SeasonId, command.Bets, command.TimeStamp);

            _repository.Save(bettor, command.Id.ToString());
        }

        public void Handle(UpdateSeasonAccount command)
        {
            var bettor = _repository.Get(command.BettorId);
            bettor.UpdateSeasonAccount(command.SeasonId, command.BookieId, command.BetTransactions);
            _repository.Save(bettor, command.Id.ToString());
        }
    }
}
