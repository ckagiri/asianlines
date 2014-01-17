using System;
using System.Diagnostics;
using Ligi.Core.Events;
using Ligi.Core.Events.Domain;
using Ligi.Infrastructure.Sql.Database;
using Ligi.Infrastructure.Sql.ReadModel;
using Microsoft.AspNet.SignalR.Client;

namespace Ligi.Infrastructure.Sql.Projections
{
    public class BookieAccountProjection : 
        IEventHandler<WeekAccountOpened>,
        IEventHandler<BetPlaced>,
        IEventHandler<BetUpdated>,
        IEventHandler<PayoutTransacted>,
        IEventHandler<BetRejected>,
        IEventHandler<BetUpdateRejected>
    {
        private readonly Func<BetsDbContext> _contextFactory;

        public BookieAccountProjection(Func<BetsDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Handle(WeekAccountOpened @event)
        {
            using (var context = _contextFactory.Invoke())
            {
                var dto = context.Find<WeekAccount>(@event.SourceId);
                if (dto != null)
                {
                    Trace.TraceWarning(
                        "Ignoring WeekAccountOpened event for account ID {0} as it's already created.",
                        @event.SourceId);
                }
                else
                {
                    dto = new WeekAccount(
                        @event.SourceId,
                        @event.UserId,
                        @event.StartDate,
                        @event.EndDate,
                        @event.SeasonId,
                        @event.Version
                        );

                    context.Save(dto);
                }
            }
        }

        public void Handle(BetPlaced @event)
        {
            UpdateDetails(@event.SourceId, @event.UserName, @event.Bet.Wager, @event.Version);
        }

        public void Handle(BetUpdated @event)
        {
            UpdateDetails(@event.SourceId, @event.UserName, @event.Bet.Wager, @event.Version);
        }

        private void UpdateDetails(Guid accountId, string userName, decimal wager, int version)
        {
            int eventVersion = version;
            int dtoVersion;
            WeekAccount dto;
            using (var context = _contextFactory.Invoke())
            {
                dto = context.Find<WeekAccount>(accountId);
                dtoVersion = dto.Version;
                if (WasNotHandled(dto, version))
                {
                    if (dto.Credit > wager)
                    {
                        dto.Credit -= wager;
                    }
                    else
                    {
                        var diff = wager - dto.Credit;
                        dto.Credit = 0;
                        dto.Balance -= diff;
                    }
                    dto.UserName = userName;
                    dto.Available = dto.Credit + dto.Balance;
                    dto.TotalStake += wager;
                    dto.Profit = dto.TotalPayout - dto.TotalStake;
                    dto.Version = version;
                    context.Save(dto);
                }
            }

            PublishWeekAccountUpdate(eventVersion, dtoVersion, dto);
        }

        public void Handle(PayoutTransacted @event)
        {
            int eventVersion = @event.Version;
            int dtoVersion;
            WeekAccount dto;
            using (var context = _contextFactory.Invoke())
            {
                dto = context.Find<WeekAccount>(@event.SourceId);
                dtoVersion = dto.Version;
                if (WasNotHandled(dto, @event.Version))
                {
                    dto.TotalPayout += @event.Bet.Payout;
                    dto.Balance += @event.Bet.Payout;
                    dto.Profit = dto.TotalPayout - dto.TotalStake;
                    dto.Available = dto.Credit + dto.Balance;
                    context.Save(dto);
                }
            }

            PublishWeekAccountUpdate(eventVersion, dtoVersion, dto);
        }

        private static bool WasNotHandled(WeekAccount account, int eventVersion)
        {
            // This assumes that events will be handled in order, but we might get the same message more than once.
            if (eventVersion > account.Version)
            {
                return true;
            }
            else if (eventVersion == account.Version)
            {
                Trace.TraceWarning(
                    "Ignoring duplicate week account update message with version {1} for account id {0}",
                    account.Id,
                    eventVersion);
                return false;
            }
            else
            {
                Trace.TraceWarning(
                    @"An older update message was received with with version {1} for account id {0}, last known version {2}.
This read model generator has an expectation that the EventBus will deliver messages for the same source in order.",
                    account.Id,
                    eventVersion,
                    account.Version);
                return false;
            }
        }

        private void PublishWeekAccountUpdate(int eventVersion, int dtoVersion, WeekAccount update)
        {
            try
            {
                if (eventVersion > dtoVersion)
                {
                    var hubConnection = new HubConnection("http://localhost:52076/");

                    IHubProxy hub = hubConnection.CreateHubProxy("ligi");

                    hubConnection.Start().Wait();
                    hub.Invoke<WeekAccount>("UpdateWeekAccount", update);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        public void Handle(BetRejected @event)
        {
            // LigiHub.BetRejected(@event.userId, @event.Bet)
        }

        public void Handle(BetUpdateRejected @event)
        {
            // LigiHub.BetUpdateRejected(@event.userId, @event.Bet)
        }
    }
}
