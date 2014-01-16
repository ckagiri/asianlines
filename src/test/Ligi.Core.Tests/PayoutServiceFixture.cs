using System;
using Ligi.Core.Model;
using Ligi.Core.Services.Impl;
using Xunit;

namespace Ligi.Core.Tests
{
    public class PayoutServiceFixture
    {
        private PayoutService _sut;
        private Bet _bet1 = new Bet
                                {
                                    FixtureId = Guid.NewGuid(),
                                    BetType = BetType.AsianHandicap,
                                    BetPick = BetPick.Home,
                                    Handicap = -3m,
                                    Stake = 200,
                                };

        private Bet _bet2 = new Bet
                                {
                                    FixtureId = Guid.NewGuid(),
                                    BetType = BetType.AsianGoals,
                                    BetPick = BetPick.Over,
                                    Handicap = 3.25m,
                                    Stake = 200m,
                                };

        public PayoutServiceFixture()
        {
            _sut = new PayoutService();
        }

        [Fact]
        public void when_calculating_hangcheng_halfball_then_payout_is_correct()
        {
            var ans = _sut.GetPayout(_bet1, 3, 0);

            Assert.Equal(BetResult.Push, ans.Result);
            Assert.Equal(200, ans.Payout);
        }

        [Fact]
        public void when_calculating_totalgoals_halfball_then_betresult_is_correct()
        {
            var ans = _sut.GetPayout(_bet2, 3, 0);

            Assert.Equal(BetResult.HalfLose, ans.Result);
            Assert.Equal(100, ans.Payout);

        }

        [Fact]
        public void rounds_to_near_2_digit_decimal()
        {
            var actual = _sut.GetPayout(_bet2, 0, 0);

            Assert.Equal(195.98m, actual.Payout);
        }
    }
}
