using System;
using Ligi.Core.DomainBase;

namespace Ligi.Core.Model
{
    // BookiesByFixture
    public class FixtureBookieIndex : IAggregateRoot
    {
        public Guid Id { get; private set; }
        public Guid FixtureId { get; set; }
        public Guid BookieId { get; set; }

        protected FixtureBookieIndex()
        { }

        public FixtureBookieIndex(Guid id, Guid fixtureId, Guid bookieId)
        {
            Id = id;
            FixtureId = fixtureId;
            BookieId = bookieId;
        }
    }
}
