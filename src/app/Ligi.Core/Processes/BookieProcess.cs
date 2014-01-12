using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Ligi.Core.Commands;
using Ligi.Core.Commands.Domain;
using Ligi.Core.Events.Domain;
using Ligi.Core.Messaging;
using Ligi.Core.Utils;

namespace Ligi.Core.Processes
{
    public class BookieProcess : IProcessManager
    {
        public BookieProcess()
        {
            Id = GuidUtil.NewSequentialId();
        }

        public enum ProcessState
        {
            NotStarted = 0,
            Running = 1,
            AwaitingSeasonAccountAck = 3,
            Completed = 2
        }
        
        public Guid Id { get; private set; }
        public Guid BettorId { get; set; }
        public Guid SeasonId { get; set; }
        public Guid BookieId { get; set; }
        public Guid UpdateSeasonAccountCommandId { get; set; }

        public bool Completed { get; private set; }
        private readonly List<Envelope<ICommand>> _commands = new List<Envelope<ICommand>>();
        public IEnumerable<Envelope<ICommand>> Commands
        {
            get { return _commands; }
        }

        public int StateValue { get; private set; }
        [NotMapped]
        public ProcessState State
        {
            get { return (ProcessState)StateValue; }
            internal set { StateValue = (int)value; }
        }
        
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] TimeStamp { get; private set; }

        public void Handle(BetslipSubmitted message)
        {
            if (State == ProcessState.NotStarted)
            {
                BettorId = message.SourceId;
                SeasonId = message.SeasonId;
                BookieId = message.BookieId;

                State = ProcessState.Running;
                AddCommand(new ProcessBets
                               {
                                   UserId = BettorId,
                                   SeasonId = SeasonId,
                                   BookieId = BookieId,
                                   UserName = message.UserName,
                                   StartDate = message.StartDate,
                                   EndDate = message.EndDate,
                                   BetslipId = message.BetslipId,
                                   Bets = message.Bets,
                               });
            }
            else if (State == ProcessState.Running)
            {
                AddCommand(new ProcessBets
                               {
                                   UserId = BettorId,
                                   SeasonId = SeasonId,
                                   BookieId = BookieId,
                                   UserName = message.UserName,
                                   StartDate = message.StartDate,
                                   EndDate = message.EndDate,
                                   BetslipId = message.BetslipId,
                                   Bets = message.Bets
                               });
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void Handle(BetsProcessed message)
        {
            if (State == ProcessState.Running)
            {
                State = ProcessState.AwaitingSeasonAccountAck;

               var updateSeasonAccountCommand =   new UpdateSeasonAccount
                               {
                                   BettorId = BettorId,
                                   BookieId = BookieId,
                                   SeasonId = SeasonId,
                                   BetTransactions = message.BetTransactions,
                               };

               UpdateSeasonAccountCommandId = updateSeasonAccountCommand.Id;
               AddCommand(new Envelope<ICommand>(updateSeasonAccountCommand));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void Handle(Envelope<SeasonAccountUpdated> envelope)
        {
            if (State == ProcessState.AwaitingSeasonAccountAck)
            {
                if (envelope.CorrelationId != null)
                {
                    if (string.CompareOrdinal(UpdateSeasonAccountCommandId.ToString(), envelope.CorrelationId) != 0)
                    {
                        // skip this event
                        Trace.TraceWarning("Season account response for bettor id {0} does not match the expected correlation id.", envelope.Body.SourceId);
                        return;
                    }
                }
                State = ProcessState.Running;
            }
            else if (string.CompareOrdinal(UpdateSeasonAccountCommandId.ToString(), envelope.CorrelationId) == 0)
            {
                Trace.TraceInformation("Season account response for request {1} for bettor id {0} was already handled. Skipping event.", envelope.Body.SourceId, envelope.CorrelationId);
            }
            else
            {
                throw new InvalidOperationException();
            }
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
