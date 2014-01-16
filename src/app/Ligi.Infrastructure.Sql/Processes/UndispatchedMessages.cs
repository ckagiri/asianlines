using System;

namespace Ligi.Infrastructure.Sql.Processes
{
    public class UndispatchedMessages
    {
        protected UndispatchedMessages() { }

        public UndispatchedMessages(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
        public string Commands { get; set; }
    }
}
