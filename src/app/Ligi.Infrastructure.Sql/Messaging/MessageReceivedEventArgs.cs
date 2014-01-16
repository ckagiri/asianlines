using System;

namespace Ligi.Infrastructure.Sql.Messaging
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(Message message)
        {
            Message = message;
        }

        public Message Message { get; private set; }
    }
}
