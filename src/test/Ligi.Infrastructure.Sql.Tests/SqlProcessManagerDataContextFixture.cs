﻿using Ligi.Core.Commands;
using Ligi.Core.Messaging;
using Ligi.Core.Processes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Ligi.Infrastructure.Sql.Processes;
using Ligi.Infrastructure.Sql.Serialization;
using Moq;
using Xunit;

namespace Ligi.Infrastructure.Sql.Tests
{
    public class SqlProcessManagerDataContextFixture : IDisposable
    {
        protected readonly string DbName = typeof(SqlProcessManagerDataContextFixture).Name + "-" + Guid.NewGuid();

        public SqlProcessManagerDataContextFixture()
        {
            using (var context = new TestProcessManagerDbContext(DbName))
            {
                context.Database.Delete();
                context.Database.Create();
            }
        }

        public void Dispose()
        {
            using (var context = new TestProcessManagerDbContext(DbName))
            {
                context.Database.Delete();
            }
        }

        [Fact]
        public void WhenSavingEntity_ThenCanRetrieveIt()
        {
            var id = Guid.NewGuid();

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), Mock.Of<ICommandBus>(), Mock.Of<ITextSerializer>()))
            {
                var conference = new OrmTestProcessManager(id);
                context.Save(conference);
            }

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), Mock.Of<ICommandBus>(), Mock.Of<ITextSerializer>()))
            {
                var conference = context.Find(id);

                Assert.NotNull(conference);
            }
        }

        [Fact]
        public void WhenSavingEntityTwice_ThenCanReloadIt()
        {
            var id = Guid.NewGuid();

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), Mock.Of<ICommandBus>(), Mock.Of<ITextSerializer>()))
            {
                var conference = new OrmTestProcessManager(id);
                context.Save(conference);
            }

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), Mock.Of<ICommandBus>(), Mock.Of<ITextSerializer>()))
            {
                var conference = context.Find(id);
                conference.Title = "CQRS Journey";

                context.Save(conference);
            }

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), Mock.Of<ICommandBus>(), Mock.Of<ITextSerializer>()))
            {
                var conference = context.Find(id);

                Assert.Equal("CQRS Journey", conference.Title);
            }
        }

        [Fact]
        public void WhenSavingWithConcurrencyConflict_ThenThrowsException()
        {
            var id = Guid.NewGuid();

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), Mock.Of<ICommandBus>(), Mock.Of<ITextSerializer>()))
            {
                var conference = new OrmTestProcessManager(id);
                context.Save(conference);
            }

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), Mock.Of<ICommandBus>(), Mock.Of<ITextSerializer>()))
            {
                var conference = context.Find(id);
                conference.Title = "CQRS Journey!";

                using (var innerContext = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), Mock.Of<ICommandBus>(), Mock.Of<ITextSerializer>()))
                {
                    var innerConference = innerContext.Find(id);
                    innerConference.Title = "CQRS Journey!!";

                    innerContext.Save(innerConference);
                }

                Assert.Throws<ConcurrencyException>(() => context.Save(conference));
            }
        }

        [Fact]
        public void WhenEntityExposesCommand_ThenRepositoryPublishesIt()
        {
            var bus = new Mock<ICommandBus>();
            var commands = new List<ICommand>();

            bus.Setup(x => x.Send(It.IsAny<Envelope<ICommand>>()))
                .Callback<Envelope<ICommand>>(x => commands.Add(x.Body));

            var command = new TestCommand();

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus.Object, Mock.Of<ITextSerializer>()))
            {
                var aggregate = new OrmTestProcessManager(Guid.NewGuid());
                aggregate.AddCommand(command);
                context.Save(aggregate);
            }

            Assert.Equal(1, commands.Count);
            Assert.True(commands.Contains(command));
        }

        [Fact]
        public void WhenCommandPublishingThrows_ThenPublishesPendingCommandOnNextFind()
        {
            var bus = new Mock<ICommandBus>();
            var command1 = new Envelope<ICommand>(new TestCommand());
            var command2 = new Envelope<ICommand>(new TestCommand());
            var id = Guid.NewGuid();

            bus.Setup(x => x.Send(command2)).Throws<TimeoutException>();

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus.Object, new JsonTextSerializer()))
            {
                var aggregate = new OrmTestProcessManager(id);
                aggregate.AddEnvelope(command1, command2);

                Assert.Throws<TimeoutException>(() => context.Save(aggregate));
            }

            bus.Verify(x => x.Send(command1));
            bus.Verify(x => x.Send(command2));


            // Clear bus for next run.
            bus = new Mock<ICommandBus>();
            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus.Object, new JsonTextSerializer()))
            {
                var aggregate = context.Find(id);

                Assert.NotNull(aggregate);
                bus.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command1.Body.Id)), Times.Never());
                bus.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Body.Id)));
            }
        }

        [Fact]
        public void WhenCommandPublishingThrowsOnFind_ThenThrows()
        {
            var bus = new Mock<ICommandBus>();
            var command1 = new Envelope<ICommand>(new TestCommand());
            var command2 = new Envelope<ICommand>(new TestCommand());
            var id = Guid.NewGuid();

            bus.Setup(x => x.Send(command2)).Throws<TimeoutException>();

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus.Object, new JsonTextSerializer()))
            {
                var aggregate = new OrmTestProcessManager(id);
                aggregate.AddEnvelope(command1, command2);

                Assert.Throws<TimeoutException>(() => context.Save(aggregate));
            }

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus.Object, new JsonTextSerializer()))
            {
                bus.Setup(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Body.Id))).Throws<TimeoutException>();

                Assert.Throws<TimeoutException>(() => context.Find(id));
            }
        }

        [Fact]
        public void WhenCommandPublishingFails_ThenThrows()
        {
            var bus = new Mock<ICommandBus>();
            var command1 = new Envelope<ICommand>(new TestCommand());
            var command2 = new Envelope<ICommand>(new TestCommand());
            var command3 = new Envelope<ICommand>(new TestCommand());
            var id = Guid.NewGuid();

            bus.Setup(x => x.Send(command2)).Throws<TimeoutException>();

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus.Object, new JsonTextSerializer()))
            {
                var aggregate = new OrmTestProcessManager(id);
                aggregate.AddEnvelope(command1, command2, command3);

                Assert.Throws<TimeoutException>(() => context.Save(aggregate));
            }
        }

        [Fact]
        public void WhenCommandPublishingThrowsPartiallyOnSave_ThenPublishesPendingCommandOnNextFind()
        {
            var bus = new Mock<ICommandBus>();
            var command1 = new Envelope<ICommand>(new TestCommand());
            var command2 = new Envelope<ICommand>(new TestCommand());
            var command3 = new Envelope<ICommand>(new TestCommand());
            var id = Guid.NewGuid();

            bus.Setup(x => x.Send(command2)).Throws<TimeoutException>();

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus.Object, new JsonTextSerializer()))
            {
                var aggregate = new OrmTestProcessManager(id);
                aggregate.AddEnvelope(command1, command2, command3);

                Assert.Throws<TimeoutException>(() => context.Save(aggregate));
            }

            bus.Verify(x => x.Send(command1));
            bus.Verify(x => x.Send(command2));
            bus.Verify(x => x.Send(command3), Times.Never());


            // Setup bus for failure only on the third deserialized command now.
            // The command2 will pass now as it's a different deserialized instance.
            bus.Setup(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Body.Id))).Throws<TimeoutException>();

            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus.Object, new JsonTextSerializer()))
            {
                Assert.Throws<TimeoutException>(() => context.Find(id));

                bus.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Body.Id)));
                bus.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Body.Id)));
            }

            // Clear bus now.
            bus = new Mock<ICommandBus>();
            using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus.Object, new JsonTextSerializer()))
            {
                var aggregate = context.Find(id);

                Assert.NotNull(aggregate);

                bus.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Body.Id)), Times.Never());
                bus.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Body.Id)));
            }
        }

        public class TestProcessManagerDbContext : DbContext
        {
            public TestProcessManagerDbContext(string nameOrConnectionString)
                : base(nameOrConnectionString)
            {
            }

            public DbSet<OrmTestProcessManager> TestProcessManagers { get; set; }
            public DbSet<UndispatchedMessages> UndispatchedMessages { get; set; }
        }

        public class TestCommand : ICommand
        {
            public TestCommand()
            {
                Id = Guid.NewGuid();
            }
            public Guid Id { get; set; }
        }
    }

    public class given_context_that_stalls_on_save_and_on_find_when_publishing_ : SqlProcessManagerDataContextFixture
    {
        protected TestCommand command1;
        protected TestCommand command2;
        protected TestCommand command3;
        protected Mock<ICommandBus> bus1;
        protected List<Exception> exceptions;
        protected ManualResetEvent saveFinished;
        protected AutoResetEvent sendContinueResetEvent1;
        protected AutoResetEvent sendStartedResetEvent1;
        protected Mock<ICommandBus> bus2;
        protected AutoResetEvent sendContinueResetEvent2;
        protected AutoResetEvent sendStartedResetEvent2;
        protected ManualResetEvent findAndSaveFinished;

        public given_context_that_stalls_on_save_and_on_find_when_publishing_()
        {
            bus1 = new Mock<ICommandBus>();
            command1 = new TestCommand();
            command2 = new TestCommand();
            command3 = new TestCommand();
            var id = Guid.NewGuid();
            exceptions = new List<Exception>();

            saveFinished = new ManualResetEvent(false);
            sendContinueResetEvent1 = new AutoResetEvent(false);
            sendStartedResetEvent1 = new AutoResetEvent(false);
            bus1.Setup(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Id)))
                .Callback(() => { sendStartedResetEvent1.Set(); sendContinueResetEvent1.WaitOne(); });

            Task.Factory.StartNew(() =>
            {
                using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus1.Object, new JsonTextSerializer()))
                {
                    var aggregate = new OrmTestProcessManager(id);
                    aggregate.AddEnvelope(new Envelope<ICommand>(command1), new Envelope<ICommand>(command2), new Envelope<ICommand>(command3));

                    context.Save(aggregate);
                }
            }).ContinueWith(t => exceptions.Add(t.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted)
            .ContinueWith(t => saveFinished.Set());

            Assert.True(sendStartedResetEvent1.WaitOne(3000));

            bus2 = new Mock<ICommandBus>();
            sendContinueResetEvent2 = new AutoResetEvent(false);
            sendStartedResetEvent2 = new AutoResetEvent(false);
            bus2.Setup(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Id)))
                .Callback(() => { sendStartedResetEvent2.Set(); sendContinueResetEvent2.WaitOne(); });

            findAndSaveFinished = new ManualResetEvent(false);

            Task.Factory.StartNew(() =>
            {
                using (var context = new SqlProcessManagerDataContext<OrmTestProcessManager>(() => new TestProcessManagerDbContext(DbName), bus2.Object, new JsonTextSerializer()))
                {
                    var entity = context.Find(id);
                    context.Save(entity);
                }
            }).ContinueWith(t => exceptions.Add(t.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted)
            .ContinueWith(t => findAndSaveFinished.Set());
        }

        [Fact]
        public void when_save_finishes_sending_first_then_find_ignores_concurrency_exception_and_refreshes_context()
        {
            Assert.True(sendStartedResetEvent2.WaitOne(3000));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command1.Id)));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Id)));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)), Times.Never());

            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command1.Id)));
            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Id)));
            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)), Times.Never());

            sendContinueResetEvent1.Set();
            Assert.True(saveFinished.WaitOne(3000));

            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)));

            sendContinueResetEvent2.Set();
            Assert.True(findAndSaveFinished.WaitOne(3000));

            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)));

            Assert.Equal(0, exceptions.Count);
        }

        [Fact]
        public void when_find_finishes_publishing_first_then_save_ignores_concurrency_exception()
        {
            Assert.True(sendStartedResetEvent2.WaitOne(3000));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command1.Id)));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Id)));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)), Times.Never());

            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command1.Id)));
            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Id)));
            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)), Times.Never());

            sendContinueResetEvent2.Set();
            Assert.True(findAndSaveFinished.WaitOne(3000));

            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)));

            sendContinueResetEvent1.Set();
            Assert.True(saveFinished.WaitOne(3000));

            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)));

            Assert.Equal(0, exceptions.Count);
        }

        [Fact]
        public void when_save_throws_sending_first_then_find_ignores_concurrency_exception_and_refreshes_context()
        {
            bus1.Setup(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id))).Throws<TimeoutException>();

            Assert.True(sendStartedResetEvent2.WaitOne(3000));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command1.Id)));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Id)));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)), Times.Never());

            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command1.Id)));
            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Id)));
            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)), Times.Never());

            sendContinueResetEvent1.Set();
            Assert.True(saveFinished.WaitOne(3000));
            
            sendContinueResetEvent2.Set();
            Assert.True(findAndSaveFinished.WaitOne(3000));

            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)));
            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)));

            Assert.Equal(1, exceptions.Count);
            Assert.IsAssignableFrom<TimeoutException>(exceptions[0]);
        }

        [Fact]
        public void when_save_throws_sending_after_find_sent_everything_then_ignores_concurrency_exception_and_surfaces_original()
        {
            bus1.Setup(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id))).Throws<TimeoutException>();

            Assert.True(sendStartedResetEvent2.WaitOne(3000));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command1.Id)));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Id)));
            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)), Times.Never());

            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command1.Id)));
            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command2.Id)));
            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)), Times.Never());

            sendContinueResetEvent2.Set();
            Assert.True(findAndSaveFinished.WaitOne(3000));

            sendContinueResetEvent1.Set();
            Assert.True(saveFinished.WaitOne(3000));

            bus1.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)));
            bus2.Verify(x => x.Send(It.Is<Envelope<ICommand>>(c => c.Body.Id == command3.Id)));

            Assert.Equal(1, exceptions.Count);
            Assert.IsAssignableFrom<TimeoutException>(exceptions[0]);
        }
    }

    public class OrmTestProcessManager : IProcessManager
    {
        private readonly List<Envelope<ICommand>> _commands = new List<Envelope<ICommand>>();

        protected OrmTestProcessManager() { }

        public OrmTestProcessManager(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

        public bool Completed { get; set; }

        public string Title { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] TimeStamp { get; private set; }

        public void AddCommand(ICommand command)
        {
            _commands.Add(Envelope.Create(command));
        }

        public void AddEnvelope(params Envelope<ICommand>[] commands)
        {
            _commands.AddRange(commands);
        }

        public IEnumerable<Envelope<ICommand>> Commands { get { return _commands; } }
    }
}
