using Ligi.Infrastructure.Messaging.Implementation;
using System;
using System.Threading;
using Ligi.Infrastructure.Sql.Messaging;
using Ligi.Infrastructure.Sql.Messaging.Implementation;
using Xunit;

namespace Ligi.Infrastructure.Sql.Tests.Messaging
{
    public class given_sender_and_receiver : IDisposable
    {
        private readonly System.Data.Entity.Infrastructure.IDbConnectionFactory _connectionFactory;
        private readonly MessageSender _sender;
        private readonly TestableMessageReceiver _receiver;

        public given_sender_and_receiver()
        {
            _connectionFactory = System.Data.Entity.Database.DefaultConnectionFactory;
            _sender = new MessageSender(_connectionFactory, "TestSqlMessaging", "Test.Commands");
            _receiver = new TestableMessageReceiver(_connectionFactory);

            MessagingDbInitializer.CreateDatabaseObjects(_connectionFactory.CreateConnection("TestSqlMessaging").ConnectionString, "Test", true);
        }

        void IDisposable.Dispose()
        {
            _receiver.Stop();

            using (var connection = _connectionFactory.CreateConnection("TestSqlMessaging"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "TRUNCATE TABLE Test.Commands";
                command.ExecuteNonQuery();
            }
        }

        [Fact]
        public void when_sending_message_then_receives_message()
        {
            Message message = null;

            _receiver.MessageReceived += (s, e) => { message = e.Message; };

            _sender.Send(new Message("test message"));

            Assert.True(_receiver.ReceiveMessage());
            Assert.Equal("test message", message.Body);
            Assert.Null(message.CorrelationId);
            Assert.Null(message.DeliveryDate);
        }

        [Fact]
        public void when_sending_message_with_correlation_id_then_receives_message()
        {
            Message message = null;

            _receiver.MessageReceived += (s, e) => { message = e.Message; };

            _sender.Send(new Message("test message", correlationId: "correlation"));

            Assert.True(_receiver.ReceiveMessage());
            Assert.Equal("test message", message.Body);
            Assert.Equal("correlation", message.CorrelationId);
            Assert.Null(message.DeliveryDate);
        }

        [Fact]
        public void when_successfully_handles_message_then_removes_message()
        {
            _receiver.MessageReceived += (s, e) => { };

            _sender.Send(new Message("test message"));

            Assert.True(_receiver.ReceiveMessage());
            Assert.False(_receiver.ReceiveMessage());
        }

        [Fact]
        public void when_unsuccessfully_handles_message_then_does_not_remove_message()
        {
            EventHandler<MessageReceivedEventArgs> failureHandler = null;
            failureHandler = (s, e) => { _receiver.MessageReceived -= failureHandler; throw new ArgumentException(); };

            _receiver.MessageReceived += failureHandler;

            _sender.Send(new Message("test message"));

            try
            {
                Assert.True(_receiver.ReceiveMessage());
                Assert.False(true, "should have thrown");
            }
            catch (ArgumentException)
            { }

            Assert.True(_receiver.ReceiveMessage());
        }

        [Fact]
        public void when_sending_message_with_delay_then_receives_message_after_delay()
        {
            Message message = null;

            _receiver.MessageReceived += (s, e) => { message = e.Message; };

            var deliveryDate = DateTime.UtcNow.Add(TimeSpan.FromSeconds(5));
            _sender.Send(new Message("test message", deliveryDate));

            Assert.False(_receiver.ReceiveMessage());

            Thread.Sleep(TimeSpan.FromSeconds(6));

            Assert.True(_receiver.ReceiveMessage());
            Assert.Equal("test message", message.Body);
        }

        [Fact]
        public void when_receiving_message_then_other_receivers_cannot_see_message_but_see_other_messages()
        {
            var secondReceiver = new TestableMessageReceiver(_connectionFactory);

            _sender.Send(new Message("message1"));
            _sender.Send(new Message("message2"));

            var waitEvent = new AutoResetEvent(false);
            string receiver1Message = null;
            string receiver2Message = null;

            _receiver.MessageReceived += (s, e) =>
            {
                waitEvent.Set();
                receiver1Message = e.Message.Body;
                waitEvent.WaitOne();
            };
            secondReceiver.MessageReceived += (s, e) =>
            {
                receiver2Message = e.Message.Body;
            };

            ThreadPool.QueueUserWorkItem(_ => { _receiver.ReceiveMessage(); });

            Assert.True(waitEvent.WaitOne(TimeSpan.FromSeconds(10)));
            secondReceiver.ReceiveMessage();
            waitEvent.Set();

            Assert.Equal("message1", receiver1Message);
            Assert.Equal("message2", receiver2Message);
        }

        [Fact]
        public void when_receiving_message_then_can_send_new_message()
        {
            var secondReceiver = new TestableMessageReceiver(_connectionFactory);

            _sender.Send(new Message("message1"));

            var waitEvent = new AutoResetEvent(false);
            string receiver1Message = null;
            string receiver2Message = null;

            _receiver.MessageReceived += (s, e) =>
            {
                waitEvent.Set();
                receiver1Message = e.Message.Body;
                waitEvent.WaitOne();
            };
            secondReceiver.MessageReceived += (s, e) =>
            {
                receiver2Message = e.Message.Body;
            };

            ThreadPool.QueueUserWorkItem(_ => { _receiver.ReceiveMessage(); });

            Assert.True(waitEvent.WaitOne(TimeSpan.FromSeconds(10)));
            _sender.Send(new Message("message2"));
            secondReceiver.ReceiveMessage();
            waitEvent.Set();

            Assert.Equal("message1", receiver1Message);
            Assert.Equal("message2", receiver2Message);
        }

        public class TestableMessageReceiver : MessageReceiver
        {
            public TestableMessageReceiver(System.Data.Entity.Infrastructure.IDbConnectionFactory connectionFactory)
                : base(connectionFactory, "TestSqlMessaging", "Test.Commands")
            {
            }

            public new bool ReceiveMessage()
            {
                return base.ReceiveMessage();
            }
        }
    }
}