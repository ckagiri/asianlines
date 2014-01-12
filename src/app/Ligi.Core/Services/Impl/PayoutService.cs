using System;
using Ligi.Core.Model;

namespace Ligi.Core.Services.Impl
{
    public class PayoutService : IPayoutService
    {
        public PayoutResult GetPayout(Bet bet, int homeScore, int awayScore)
        {
            const decimal odds = 1.95m;
            var handicap = bet.Handicap;
            var mod = (int) ((handicap*4)%2);
            var wager = bet.Stake;
            int diff;
            decimal payout = 0;
            var result = BetResult.Void;
            
            if (bet.BetType == BetType.AsianHandicap)
            {
                if (bet.BetPick == BetPick.Home)
                {
                    diff = -(homeScore - awayScore);
                    result = DetermineResult(diff, handicap, mod);
                    payout = DeterminePayout(diff, handicap, mod, odds, wager);
                }
                else if (bet.BetPick == BetPick.Away)
                {
                    diff = -(awayScore - homeScore);
                    result = DetermineResult(diff, handicap, mod);
                    payout = DeterminePayout(diff, handicap, mod, odds, wager);
                }
                else
                {
                    payout = bet.Stake;
                }
            }
            else if (bet.BetType == BetType.AsianGoals)
            {
                handicap = -handicap;
                if (bet.BetPick == BetPick.Over)
                {
                    diff = -(homeScore + awayScore);
                    result = DetermineResult(diff, handicap, mod);
                    payout = DeterminePayout(diff, handicap, mod, odds, wager);
                }
                else if (bet.BetPick == BetPick.Under)
                {
                    diff = homeScore + awayScore;
                    result = DetermineResult(diff, handicap, mod);
                    payout = DeterminePayout(diff, handicap, mod, odds, wager);
                }
            }
            else
            {
                payout =  bet.Stake;
            }
            return new PayoutResult { Result = result, Payout = payout };
        }

        private static BetResult DetermineResult(int diff, decimal handicap, int mod)
        {
            diff = -diff;
            if (mod == 0) 
            {
                if ((diff + handicap) > 0) {
                    return BetResult.Win;
                } 
                else if ((diff + handicap) == 0) 
                {
                    return BetResult.Push;
                } 
                else 
                {
                    return BetResult.Lose;
                }
            } 
            else 
            {
                var compareUp = diff + handicap + 0.25m;
                var compareDown = diff + handicap - 0.25m;
                if ((compareUp > 0) && (compareDown > 0)) 
                {
                    return BetResult.Win;
                } 
                else if ((compareUp < 0) && (compareDown < 0)) 
                {
                    return BetResult.Lose;
                } 
                else if ((compareUp > 0) && (compareDown == 0)) 
                {
                    return BetResult.HalfWin;
                } 
                else 
                {
                    return BetResult.HalfLose;
                }
            }
        }

        private static decimal DeterminePayout(int diff, decimal handicap, int mod, decimal odds, decimal wager)
        {
            diff = -diff;
            decimal payout;
            if (mod == 0)
            {
                if ((diff + handicap) > 0)
                {
                    payout = wager + (wager * (odds - 1));
                }
                else if ((diff + handicap) == 0)
                {
                    payout = wager;
                }
                else
                {
                    payout = 0;
                }
            }
            else
            {
                var compareUp = (diff + handicap) + 0.25m;
                var compareDown = (diff + handicap) - 0.25m;
                if ((compareUp > 0) && (compareDown > 0))
                {
                    payout = wager + (wager * (odds - 1));
                }
                else if ((compareUp < 0) && (compareDown < 0))
                {
                    payout = 0;
                }
                else if ((compareUp > 0) && (compareDown == 0))
                {
                    payout = wager + (0.5m * (wager * (odds - 1)));
                }
                else
                {
                    payout = wager / 2;
                }
            }

            return Math.Round(payout, 2);
        }
    }
}
