using Ligi.Core.Model;

namespace Ligi.Core.Services
{
    public interface IPayoutService
    {
        PayoutResult GetPayout(Bet bet, int homeScore, int awayScore);
    }

    public class PayoutResult
    {
        public BetResult Result { get; set; }
        public decimal Payout { get; set; }
    }
}
