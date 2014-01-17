using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ligi.Core.DomainBase;
using Ligi.Core.Events;
using Ligi.Core.Messaging;
using Ligi.Infrastructure.Sql.Database;
using Moq;
using Xunit;

namespace Ligi.Infrastructure.Sql.Tests
{
    public class SqlDataContextFixture : IDisposable
    {
        public SqlDataContextFixture()
        {
            using (var dbContext = new TestDbContext())
            {
                dbContext.Database.Delete();
                dbContext.Database.Create();
            }
        }

        public void Dispose()
        {
            using (var dbContext = new TestDbContext())
            {
                dbContext.Database.Delete();
            }
        }

        [Fact]
        public void WhenSavingAggregateRoot_ThenCanRetrieveIt()
        {
            var id = Guid.NewGuid();

            using (var context = new SqlDataContext<TestAggregateRoot>(() => new TestDbContext(), Mock.Of<IEventBus>()))
            {
                var aggregateRoot = new TestAggregateRoot(id) { Title = "test" };

                context.Save(aggregateRoot);
            }

            using (var context = new SqlDataContext<TestAggregateRoot>(() => new TestDbContext(), Mock.Of<IEventBus>()))
            {
                var aggregateRoot = context.Find(id);

                Assert.NotNull(aggregateRoot);
                Assert.Equal("test", aggregateRoot.Title);
            }
        }

        [Fact]
        public void WhenSavingEntityTwice_ThenCanReloadIt()
        {
            var id = Guid.NewGuid();

            using (var context = new SqlDataContext<TestAggregateRoot>(() => new TestDbContext(), Mock.Of<IEventBus>()))
            {
                var aggregateRoot = new TestAggregateRoot(id);
                context.Save(aggregateRoot);
            }

            using (var context = new SqlDataContext<TestAggregateRoot>(() => new TestDbContext(), Mock.Of<IEventBus>()))
            {
                var aggregateRoot = context.Find(id);
                aggregateRoot.Title = "test";

                context.Save(aggregateRoot);
            }

            using (var context = new SqlDataContext<TestAggregateRoot>(() => new TestDbContext(), Mock.Of<IEventBus>()))
            {
                var aggregateRoot = context.Find(id);

                Assert.Equal("test", aggregateRoot.Title);
            }
        }

        [Fact]
        public void WhenEntityExposesEvent_ThenRepositoryPublishesIt()
        {
            var busMock = new Mock<IEventBus>();
            var events = new List<IEvent>();

            busMock
                .Setup(x => x.Publish(It.IsAny<IEnumerable<Envelope<IEvent>>>()))
                .Callback<IEnumerable<Envelope<IEvent>>>(x => events.AddRange(x.Select(e => e.Body)));

            var @event = new TestEvent();

            using (var context = new SqlDataContext<TestEventPublishingAggregateRoot>(() => new TestDbContext(), busMock.Object))
            {
                var aggregate = new TestEventPublishingAggregateRoot(Guid.NewGuid());
                aggregate.AddEvent(@event);
                context.Save(aggregate);
            }

            Assert.Equal(1, events.Count);
            Assert.True(events.Contains(@event));
        }

        public class TestDbContext : DbContext
        {
            public TestDbContext()
                : base("TestDbContext")
            {
            }

            public DbSet<TestAggregateRoot> TestAggregateRoots { get; set; }

            public DbSet<TestEventPublishingAggregateRoot> TestEventPublishingAggregateRoot { get; set; }
        }

        public class TestEvent : IEvent
        {
            public Guid SourceId { get; set; }
        }
    }

    public class TestAggregateRoot : IAggregateRoot
    {
        protected TestAggregateRoot() { }

        public TestAggregateRoot(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class TestEventPublishingAggregateRoot : TestAggregateRoot, IEventPublisher
    {
        private readonly List<IEvent> _events = new List<IEvent>();

        protected TestEventPublishingAggregateRoot() { }

        public TestEventPublishingAggregateRoot(Guid id)
            : base(id)
        {
        }

        public void AddEvent(IEvent @event)
        {
            _events.Add(@event);
        }

        public IEnumerable<IEvent> Events { get { return _events; } }
    }
}
