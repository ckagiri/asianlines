using Ligi.Infrastructure.Messaging.Implementation;
using System;
using System.Data.Entity.Infrastructure;
using Ligi.Infrastructure.Sql.Messaging;
using Ligi.Infrastructure.Sql.Messaging.Implementation;
using Xunit;

namespace Ligi.Infrastructure.Sql.Tests.Messaging
{
    public class given_sender : IDisposable
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly MessageSender _sender;

        public given_sender()
        {
            _connectionFactory = System.Data.Entity.Database.DefaultConnectionFactory;
            _sender = new MessageSender(_connectionFactory, "TestSqlMessaging", "Test.Commands");

            MessagingDbInitializer.CreateDatabaseObjects(_connectionFactory.CreateConnection("TestSqlMessaging").ConnectionString, "Test", true);
        }

        void IDisposable.Dispose()
        {
            using (var connection = _connectionFactory.CreateConnection("TestSqlMessaging"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "TRUNCATE TABLE Test.Commands";
                command.ExecuteNonQuery();
            }
        }

        [Fact]
        public void when_sending_string_message_then_saves_message()
        {
            var messageBody = "Message-" + Guid.NewGuid().ToString();
            var message = new Message(messageBody);

            _sender.Send(message);

            //using (var context = _contextFactory())
            //{
            //    Assert.True(context.Set<Message>().Any(m => m.Body.Contains(messageBody)));
            //}
        }
    }
}
