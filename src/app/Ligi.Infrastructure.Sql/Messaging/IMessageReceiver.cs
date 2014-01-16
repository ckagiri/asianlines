using System;
using Ligi.Infrastructure.Messaging;

namespace Ligi.Infrastructure.Sql.Messaging
{
    public interface IMessageReceiver
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        void Start();
        void Stop();
    }
}
