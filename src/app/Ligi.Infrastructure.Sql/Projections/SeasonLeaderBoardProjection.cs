using System;
using System.Diagnostics;
using Ligi.Core.Events;
using Ligi.Core.Events.Domain;
using Ligi.Infrastructure.Sql.Database;
using Ligi.Infrastructure.Sql.ReadModel;

namespace Ligi.Infrastructure.Sql.Projections
{
    public class SeasonLeaderBoardProjection :
        IEventHandler<SeasonAccountUpdated>
    {
        private readonly Func<BetsDbContext> _contextFactory;

        public SeasonLeaderBoardProjection(Func<BetsDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Handle(SeasonAccountUpdated @event)
        {
            using (var context = _contextFactory.Invoke())
            {
                var dto = context.Find<SeasonLeaderBoard>(@event.SeasonAccount.Id);
                if (dto == null)
                {
                    dto = new SeasonLeaderBoard
                              {
                                  Id = @event.SeasonAccount.Id,
                                  UserId = @event.SourceId,
                                  UserName = @event.SeasonAccount.UserName,
                                  SeasonId = @event.SeasonAccount.SeasonId,
                                  TotalPayout = @event.SeasonAccount.TotalPayout,
                                  TotalStake = @event.SeasonAccount.TotalStake,
                                  Profit = @event.SeasonAccount.Profit,
                                  BetsPlaced = @event.SeasonAccount.BetsPlaced,
                                  BetsSettled = @event.SeasonAccount.BetsSettled,
                                  LatestBetTimeStamp = @event.SeasonAccount.LatestBetTimeStamp,
                                  Version = @event.Version
                              };
                }
                else
                {
                    if (WasNotHandled(dto, @event.Version))
                    {
                        dto.UserName = @event.SeasonAccount.UserName;
                        dto.TotalPayout = @event.SeasonAccount.TotalPayout;
                        dto.TotalStake = @event.SeasonAccount.TotalStake;
                        dto.Profit = @event.SeasonAccount.Profit;
                        dto.BetsPlaced = @event.SeasonAccount.BetsPlaced;
                        dto.BetsSettled = @event.SeasonAccount.BetsSettled;
                        dto.LatestBetTimeStamp = @event.SeasonAccount.LatestBetTimeStamp;
                        dto.Version = @event.Version;
                    }
                }

                context.Save(dto);
            }
        }

        private static bool WasNotHandled(SeasonLeaderBoard entry, int eventVersion)
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
