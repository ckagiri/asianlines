using System;
using System.Collections.Generic;
using Ligi.Core.DataAccess;
using Ligi.Core.Events.Domain;
using Ligi.Core.Model;
using Ligi.Core.Processes;
using Moq;
using Xunit;

namespace Ligi.Core.Tests
{
    public class BetProcessRouterFixture
    {
        private static readonly Guid SeasonId = Guid.NewGuid();
        private static readonly Guid FixtureId = Guid.NewGuid();
        private static readonly Guid BookieId1 = Guid.NewGuid();
        private static readonly Guid BookieId2 = Guid.NewGuid();

        private MatchResultConfirmed _result = new MatchResultConfirmed
                                                   {
                                                       SourceId = SeasonId,
                                                       FixtureId = FixtureId,
                                                       MatchStatus = MatchStatus.Played,
                                                       HomeScore = 0,
                                                       AwayScore = 0
                                                   };

        public void when_match_result_confirmed_then_routes_approp_commands_and_saves()
        {
            var bp = new BetProcess
                         {
                             FixtureId = FixtureId
                         };
            var context = new StubProcessManagerDataContext<BetProcess> { Store = { bp } };
            var fixtureBookiesDao = new Mock<IFixtureBookiesDao>();
            fixtureBookiesDao.Setup(x => x.GetBookies(FixtureId)).Returns(new List<Guid>
                                                                              {
                                                                                  BookieId1, BookieId2
                                                                              });

            var router = new BetProcessRouter(() => context, fixtureBookiesDao.Object);

            router.Handle(_result);

            Assert.Equal(1, context.SavedProcesses.Count);

        }
    }
}
