using Ligi.Core.Commands;
using Ligi.Core.Events;

namespace Ligi.Infrastructure.Sql.MessageLog
{
    public class SqlMessageLogHandler : IEventHandler<IEvent>, ICommandHandler<ICommand>
    {
        private readonly SqlMessageLog _log;

        public SqlMessageLogHandler(SqlMessageLog log)
        {
            _log = log;
        }

        public void Handle(IEvent @event)
        {
            _log.Save(@event);
        }

        public void Handle(ICommand command)
        {
            _log.Save(command);
        }
    }
}
