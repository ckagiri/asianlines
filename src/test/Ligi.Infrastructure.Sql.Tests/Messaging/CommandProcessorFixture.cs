using System;
using System.IO;
using Ligi.Core.Commands;
using Ligi.Infrastructure.Messaging.Handling;
using Ligi.Infrastructure.Sql.Messaging;
using Ligi.Infrastructure.Sql.Serialization;
using Moq;
using Xunit;

namespace Ligi.Infrastructure.Sql.Tests.Messaging
{
    public class given_command_processor
    {
        private readonly Mock<IMessageReceiver> _receiverMock;
        private readonly CommandProcessor _processor;

        public given_command_processor()
        {
            _receiverMock = new Mock<IMessageReceiver>();
            _processor = new CommandProcessor(_receiverMock.Object, CreateSerializer());
        }

        [Fact]
        public void when_starting_then_starts_receiver()
        {
            _processor.Start();

            _receiverMock.Verify(r => r.Start());
        }

        [Fact]
        public void when_stopping_after_starting_then_stops_receiver()
        {
            _processor.Start();
            _processor.Stop();

            _receiverMock.Verify(r => r.Stop());
        }

        [Fact]
        public void when_receives_message_then_notifies_registered_handler()
        {
            var handlerAMock = new Mock<ICommandHandler>();
            handlerAMock.As<ICommandHandler<Command1>>();

            var handlerBMock = new Mock<ICommandHandler>();
            handlerBMock.As<ICommandHandler<Command2>>();

            _processor.Register(handlerAMock.Object);
            _processor.Register(handlerBMock.Object);

            _processor.Start();

            var command1 = new Command1 { Id = Guid.NewGuid() };
            var command2 = new Command2 { Id = Guid.NewGuid() };

            _receiverMock.Raise(r => r.MessageReceived += null, new MessageReceivedEventArgs(new Message(Serialize(command1))));
            _receiverMock.Raise(r => r.MessageReceived += null, new MessageReceivedEventArgs(new Message(Serialize(command2))));

            handlerAMock.As<ICommandHandler<Command1>>().Verify(h => h.Handle(It.Is<Command1>(e => e.Id == command1.Id)));
            handlerBMock.As<ICommandHandler<Command2>>().Verify(h => h.Handle(It.Is<Command2>(e => e.Id == command2.Id)));
        }

        private static string Serialize(object payload)
        {
            var serializer = CreateSerializer();

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, payload);
                return writer.ToString();
            }
        }

        private static ITextSerializer CreateSerializer()
        {
            return new JsonTextSerializer();
        }

        public class Command1 : ICommand
        {
            public Guid Id { get; set; }
        }

        public class Command2 : ICommand
        {
            public Guid Id { get; set; }
        }
    }
}
