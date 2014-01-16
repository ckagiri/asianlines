using System;

namespace Ligi.Infrastructure.Sql.Messaging
{
    public class Message
    {
        public Message(string body, DateTime? deliveryDate = null, string correlationId = null)
        {
            Body = body;
            DeliveryDate = deliveryDate;
            CorrelationId = correlationId;
        }

        public string Body { get; private set; }

        public string CorrelationId { get; private set; }

        public DateTime? DeliveryDate { get; private set; }
    }
}
