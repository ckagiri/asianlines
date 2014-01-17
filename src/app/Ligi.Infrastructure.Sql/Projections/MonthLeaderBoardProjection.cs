using System;
using System.Diagnostics;
using Ligi.Core.Events;
using Ligi.Core.Events.Domain;
using Ligi.Infrastructure.Sql.Database;
using Ligi.Infrastructure.Sql.ReadModel;

namespace Ligi.Infrastructure.Sql.Projections
{
    public class MonthLeaderBoardProjection
        : IEventHandler<MonthAccountUpdated>
    {
        private readonly Func<BetsDbContext> _contextFactory;

        public MonthLeaderBoardProjection(Func<BetsDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Handle(MonthAccountUpdated @event)
        {
            using (var context = _contextFactory.Invoke())
            {
                var dto = context.Find<MonthLeaderBoard>(@event.MonthAccount.Id);
                if (dto == null)
                {
                    dto = new MonthLeaderBoard
                                {
                                    Id = @event.MonthAccount.Id,
                                    UserId = @event.SourceId,
                                    UserName = @event.MonthAccount.UserName,
                                    SeasonId = @event.MonthAccount.SeasonId,
                                    Year = @event.MonthAccount.Year,
                                    Month = @event.MonthAccount.Month,
                                    TotalPayout = @event.MonthAccount.TotalPayout,
                                    TotalStake = @event.MonthAccount.TotalStake,
                                    Profit = @event.MonthAccount.Profit,
                                    BetsPlaced = @event.MonthAccount.BetsPlaced,
                                    BetsSettled = @event.MonthAccount.BetsSettled,
                                    LatestBetTimeStamp = @event.MonthAccount.LatestBetTimeStamp,
                                    Version = @event.Version
                                };
                }
                else
                {
                    if(WasNotHandled(dto, @event.Version))
                    {
                        dto.UserName = @event.MonthAccount.UserName;
                        dto.TotalPayout = @event.MonthAccount.TotalPayout;
                        dto.TotalStake = @event.MonthAccount.TotalStake;
                        dto.Profit = @event.MonthAccount.Profit;
                        dto.BetsPlaced = @event.MonthAccount.BetsPlaced;
                        dto.BetsSettled = @event.MonthAccount.BetsSettled;
                        dto.LatestBetTimeStamp = @event.MonthAccount.LatestBetTimeStamp;
                        dto.Version = @event.Version;
                    }
                }

                context.Save(dto);
            }
        }

        private static bool WasNotHandled(MonthLeaderBoard entry, int eventVersion)
        {
            // This assumes that events will be handled in order, but we might get the same message more than once.
            if (eventVersion > entry.Version)
            {
                return true;
            }
            else if (eventVersion == entry.Version)
            {
                Trace.TraceWarning(
                    "Ignoring duplicate leader board update message with version {1} for id {0}",
                    entry.Id,
                    eventVersion);
                return false;
            }
            else
            {
                Trace.TraceWarning(
                    @"An older update message was received with with version {1} for entry id {0}, last known version {2}.
This read model generator has an expectation that the EventBus will deliver messages for the same source in order.",
                    entry.Id,
                    eventVersion,
                    entry.Version);
                return false;
            }
        }
    }
}
