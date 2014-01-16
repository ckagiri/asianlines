using System.Collections.Generic;
using Ligi.Infrastructure.Messaging;

namespace Ligi.Infrastructure.Sql.Messaging
{
    public interface IMessageSender
    {
        void Send(Message message);
        void Send(IEnumerable<Message> messages);
    }
}
