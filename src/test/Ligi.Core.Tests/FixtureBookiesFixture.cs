using System;
using Ligi.Core.Commands.Domain;
using Ligi.Core.DataAccess;
using Ligi.Core.Handlers;
using Ligi.Core.Model;
using Moq;
using Xunit;

namespace Ligi.Core.Tests
{
    public class given_no_bookies
    {
        private readonly Mock<IDataContext<FixtureBookieIndex>> _contextMock;
        private FixtureBookiesHandler _handler;

        public given_no_bookies()
        {
            _contextMock = new Mock<IDataContext<FixtureBookieIndex>>();
            _handler = new FixtureBookiesHandler(() => _contextMock.Object);

        }

        [Fact]
        public void when_adding_new_fixture_bookie_index_then_adds_index()
        {
            FixtureBookieIndex index = null;
            Guid fixtureId = Guid.NewGuid();
            Guid bookieId = Guid.NewGuid();

            _contextMock.Setup(x => x.Save(It.IsAny<FixtureBookieIndex>()))
                .Callback<FixtureBookieIndex>(i => index = i);

            _handler.Handle(new AddBookie
                                {
                                    FixtureId = fixtureId,
                                    BookieId = bookieId
                                });

            Assert.NotNull(index);
        }
    }

    public class given_a_bookie_has_been_added_to_a_fixture
    {
        private readonly Mock<IDataContext<FixtureBookieIndex>> _contextMock;
        private readonly FixtureBookieIndex _fixtureBookieIndex;
        private FixtureBookiesHandler _handler;
        private Guid id = Guid.NewGuid();
        private Guid fixtureId = Guid.NewGuid();
        private Guid bookieId = Guid.NewGuid();


        public given_a_bookie_has_been_added_to_a_fixture()
        {
            _contextMock = new Mock<IDataContext<FixtureBookieIndex>>();
            _fixtureBookieIndex = new FixtureBookieIndex(id, fixtureId, bookieId);
            _handler = new FixtureBookiesHandler(() => _contextMock.Object);

            _contextMock.Setup(x => x.Find(_fixtureBookieIndex.Id))
                .Returns(_fixtureBookieIndex);
        }

        [Fact]
        public void when_adding_the_same_bookie_to_the_same_fixture_then_its_not_added()
        {
            _handler.Handle(new AddBookie(id)
                                {
                                   FixtureId = fixtureId,
                                   BookieId = bookieId
                                });

            _contextMock.Verify(r => r.Save(_fixtureBookieIndex), Times.Never());
        }
    }
}
