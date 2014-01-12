using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ligi.Core.Commands;
using Ligi.Core.Commands.Domain;
using Ligi.Core.Events.Domain;
using Ligi.Core.Messaging;
using Ligi.Core.Utils;

namespace Ligi.Core.Processes
{
    public class BetProcess : IProcessManager
    {
        public BetProcess()
        {
            Id = GuidUtil.NewSequentialId();
        }

        public BetProcess(Guid fixtureId) : this()
        {
            FixtureId = fixtureId;
        }

        public Guid Id { get; private set; }
        public bool Completed { get; private set; }
        [NotMapped]
        public IEnumerable<Guid> Bookies { get; set; }        
        private readonly List<Envelope<ICommand>> _commands = new List<Envelope<ICommand>>();
        public IEnumerable<Envelope<ICommand>> Commands
        {
            get { return _commands; }
        }
        public Guid FixtureId { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] TimeStamp { get; private set; }
       
        public void Handle(BetPlaced message)
        {
            if (!Completed)
            {
                var bookieId = message.SourceId;
                AddCommand(new AddBookie { FixtureId = FixtureId, BookieId = bookieId });
            }
        }

        public void Handle(MatchResultConfirmed message)
        {
            foreach (var bookie in Bookies)
            {
                AddCommand(new TransactPayout
                               {
                                   BookieId = bookie,
                                   FixtureId = FixtureId,
                                   MatchStatus = message.MatchStatus,
                                   HomeScore = message.HomeScore,
                                   AwayScore = message.AwayScore
                               });
            }
            Completed = true;
        }

        private void AddCommand<T>(T command)
            where T : ICommand
        {
            _commands.Add(Envelope.Create<ICommand>(command));
        }

        private void AddCommand(Envelope<ICommand> envelope)
        {
            _commands.Add(envelope);
        }
    }
}
