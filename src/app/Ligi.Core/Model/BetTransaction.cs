namespace Ligi.Core.Model
{
    public class BetTransaction
    {
        public TransactionType TxType { get; set; }
        public Bet Bet { get; set; }
    }

    public enum TransactionType
    {
        New,
        Update,
        Payout
    }
}
