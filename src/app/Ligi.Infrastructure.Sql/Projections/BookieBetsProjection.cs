using System;
using System.Diagnostics;
using Ligi.Core.Events;
using Ligi.Core.Events.Domain;
using Ligi.Infrastructure.Sql.Database;
using Ligi.Infrastructure.Sql.ReadModel;
using Microsoft.AspNet.SignalR.Client;

namespace Ligi.Infrastructure.Sql.Projections
{
    public class BookieBetsProjection :
        IEventHandler<BetPlaced>,
        IEventHandler<BetUpdated>,
        IEventHandler<PayoutTransacted>
    {
        private readonly Func<BetsDbContext> _contextFactory;

        public BookieBetsProjection(Func<BetsDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Handle(BetPlaced @event)
        {
            Bet dto;
            using (var context = _contextFactory.Invoke())
            {
                dto = context.Find<Bet>(@event.Bet.Id);
                if (dto == null)
                {
                    dto = new Bet
                              {
                                  Id = @event.Bet.Id, 
                                  SeasonId = @event.SeasonId,
                                  FixtureId = @event.Bet.FixtureId,
                                  BetType = @event.Bet.BetType,
                                  BetPick = @event.Bet.BetPick,
                                  Handicap = @event.Bet.Handicap,
                                  Stake = @event.Bet.Stake,
                                  UserId = @event.UserId,
                                  BookieId = @event.SourceId,
                                  TimeStamp = @event.Bet.TimeStamp,
                                  Version = @event.Version
                              };
                    context.Save(dto);
                }
                else
                {
                    Trace.TraceWarning(
                        "Ignoring duplicate bet-placed message with version {1} for bettor id {0}",
                    @event.UserId,
                    @event.Version);
                }
            }

            PublishBet(dto);
        }

        public void Handle(BetUpdated @event)
        {
            Bet dto;
            int eventVersion = @event.Version;
            int dtoVersion;
            using (var context = _contextFactory.Invoke())
            {
                dto = context.Find<Bet>(@event.Bet.Id);
                dtoVersion = dto.Version;
                if (WasNotHandled(dto, @event.Version))
                {
                    dto.Stake = @event.Bet.Stake;
                    dto.Version = @event.Version;
                    context.Save(dto);
                }
            }

            PublishBet(eventVersion, dtoVersion, dto);
        }

        public void Handle(PayoutTransacted @event)
        {
            Bet dto;
            int eventVersion = @event.Version;
            int dtoVersion;
            using (var context = _contextFactory.Invoke())
            {
                dto = context.Find<Bet>(@event.Bet.Id);
                dtoVersion = dto.Version;
                if (WasNotHandled(dto, @event.Version))
                {
                    dto.Payout = @event.Bet.Payout;
                    dto.Profit = @event.Bet.Profit;
                    dto.BetResult = @event.BetResult;
                    dto.Version = @event.Version;
                    context.Save(dto);
                }
            }

            PublishBet(eventVersion, dtoVersion, dto);
        }

        private static bool WasNotHandled(Bet bet, int eventVersion)
        {
            // This assumes that events will be handled in order, but we might get the same message more than once.
            if (eventVersion > bet.Version)
            {
                return true;
            }
            else if (eventVersion == bet.Version)
            {
                Trace.TraceWarning(
                    "Ignoring duplicate bet-update message with version {1} for bettor id {0}",
                    bet.UserId,
                    eventVersion);
                return false;
            }
            else
            {
                Trace.TraceWarning(
                    @"An older order update message was received with with version {1} for bettor id {0}, last known version {2}.
This read model generator has an expectation that the EventBus will deliver messages for the same source in order.",
                    bet.UserId,
                    eventVersion,
                    bet.Version);
                return false;
            }
        }

        private void PublishBet(int eventVersion, int dtoVersion, Bet bet)
        {
            if (eventVersion > dtoVersion)
            {
                PublishBet(bet);
            }
        }

        private void PublishBet(Bet bet)
        {
            try
            {
                var hubConnection = new HubConnection("http://localhost:52076/");
                
                IHubProxy hub = hubConnection.CreateHubProxy("ligi");
                hubConnection.Start().Wait();
                hub.Invoke<Bet>("UpsertBet", bet);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }
    }
}
