using System;
using Ligi.Core.DataAccess;
using Ligi.Core.Events;
using Ligi.Core.Events.Domain;

namespace Ligi.Core.Processes
{
    public class BetProcessRouter : 
        IEventHandler<BetPlaced>,   
        IEventHandler<MatchResultConfirmed>
    {
        private readonly Func<IProcessManagerDataContext<BetProcess>> _contextFactory;
        private readonly IFixtureBookiesDao _fixtureBookiesDao;

        public BetProcessRouter(
            Func<IProcessManagerDataContext<BetProcess>> contextFactory, IFixtureBookiesDao fixtureBookiesDao)
        {
            _contextFactory = contextFactory;
            _fixtureBookiesDao = fixtureBookiesDao;
        }

        public void Handle(BetPlaced @event)
        {
            using (var context = _contextFactory.Invoke())
            {
                var fixtureId = @event.Bet.FixtureId;
                var p = context.Find(x => x.FixtureId == fixtureId);
                if (p == null)
                {
                    p = new BetProcess(fixtureId);
                }
                p.Handle(@event);
                context.Save(p);
            }
        }

        public void Handle(MatchResultConfirmed @event)
        {
            using (var context = _contextFactory.Invoke())
            {
                var p = context.Find(x => x.FixtureId == @event.FixtureId);
                if (p != null)
                {
                    p.Bookies = _fixtureBookiesDao.GetBookies(@event.FixtureId);
                    p.Handle(@event);
                    context.Save(p);
                }
            }
        }
    }
}
