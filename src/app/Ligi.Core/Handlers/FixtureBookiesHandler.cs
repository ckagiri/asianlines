using System;
using System.Diagnostics;
using Ligi.Core.Commands;
using Ligi.Core.Commands.Domain;
using Ligi.Core.DataAccess;
using Ligi.Core.Model;

namespace Ligi.Core.Handlers
{
    public class FixtureBookiesHandler : ICommandHandler<AddBookie>
    {
        private readonly Func<IDataContext<FixtureBookieIndex>> _contextFactory;

        public FixtureBookiesHandler(Func<IDataContext<FixtureBookieIndex>> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Handle(AddBookie command)
        {
            var repository = _contextFactory();
            using (repository)
            {
                var fixtureBookieIndex = repository.Find(i =>
                    i.FixtureId == command.FixtureId && i.BookieId == command.BookieId);

                if (fixtureBookieIndex == null)
                {
                    fixtureBookieIndex = new FixtureBookieIndex(command.Id, command.FixtureId, command.BookieId);
                    repository.Save(fixtureBookieIndex);
                }
                else
                {
                    Trace.TraceError("Ignoring command {0} because bookie was already added.", command.Id);
                }
            }
        }
    }
}
