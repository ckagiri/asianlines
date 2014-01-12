using System;
using Ligi.Core.Events;
using Ligi.Core.Messaging;
using Moq;
using Xunit;

namespace Ligi.Core.Tests.Messaging.Handling
{
    public class given_empty_dispatcher
    {
        private readonly EventDispatcher _sut;

        public given_empty_dispatcher()
        {
            _sut = new EventDispatcher();
        }

        [Fact]
        public void when_dispatching_an_event_then_does_nothing()
        {
            var @event = new EventC();

            _sut.DispatchMessage(@event, "message", "correlation", "");
        }
    }

    public class given_dispatcher_with_handler
    {
        private readonly EventDispatcher _sut;
        private readonly Mock<IEventHandler> _handlerMock;

        public given_dispatcher_with_handler()
        {
            _sut = new EventDispatcher();

            _handlerMock = new Mock<IEventHandler>();
            _handlerMock.As<IEventHandler<EventA>>();

            _sut.Register(_handlerMock.Object);
        }

        [Fact]
        public void when_dispatching_an_event_with_registered_handler_then_invokes_handler()
        {
            var @event = new EventA();

            _sut.DispatchMessage(@event, "message", "correlation", "");

            _handlerMock.As<IEventHandler<EventA>>().Verify(h => h.Handle(@event), Times.Once());
        }

        [Fact]
        public void when_dispatching_an_event_with_no_registered_handler_then_does_nothing()
        {
            var @event = new EventC();

            _sut.DispatchMessage(@event, "message", "correlation", "");
        }
    }

    public class given_dispatcher_with_handler_for_envelope
    {
        private readonly EventDispatcher _sut;
        private readonly Mock<IEventHandler> _handlerMock;

        public given_dispatcher_with_handler_for_envelope()
        {
            _sut = new EventDispatcher();

            _handlerMock = new Mock<IEventHandler>();
            _handlerMock.As<IEnvelopedEventHandler<EventA>>();

            _sut.Register(_handlerMock.Object);
        }

        [Fact]
        public void when_dispatching_an_event_with_registered_handler_then_invokes_handler()
        {
            var @event = new EventA();

            _sut.DispatchMessage(@event, "message", "correlation", "");

            _handlerMock.As<IEnvelopedEventHandler<EventA>>()
                .Verify(
                    h => h.Handle(It.Is<Envelope<EventA>>(e => e.Body == @event && e.MessageId == "message" && e.CorrelationId == "correlation")),
                    Times.Once());
        }

        [Fact]
        public void when_dispatching_an_event_with_no_registered_handler_then_does_nothing()
        {
            var @event = new EventC();

            _sut.DispatchMessage(@event, "message", "correlation", "");
        }
    }

    public class given_dispatcher_with_multiple_handlers
    {
        private readonly EventDispatcher _sut;
        private readonly Mock<IEventHandler> _handler1Mock;
        private readonly Mock<IEventHandler> _handler2Mock;

        public given_dispatcher_with_multiple_handlers()
        {
            _sut = new EventDispatcher();

            _handler1Mock = new Mock<IEventHandler>();
            _handler1Mock.As<IEnvelopedEventHandler<EventA>>();
            _handler1Mock.As<IEventHandler<EventB>>();

            _sut.Register(_handler1Mock.Object);

            _handler2Mock = new Mock<IEventHandler>();
            _handler2Mock.As<IEventHandler<EventA>>();

            _sut.Register(_handler2Mock.Object);
        }

        [Fact]
        public void when_dispatching_an_event_with_multiple_registered_handlers_then_invokes_handlers()
        {
            var @event = new EventA();

            _sut.DispatchMessage(@event, "message", "correlation", "");

            _handler1Mock.As<IEnvelopedEventHandler<EventA>>()
                .Verify(
                    h => h.Handle(It.Is<Envelope<EventA>>(e => e.Body == @event && e.MessageId == "message" && e.CorrelationId == "correlation")),
                    Times.Once());
            _handler2Mock.As<IEventHandler<EventA>>().Verify(h => h.Handle(@event), Times.Once());
        }

        [Fact]
        public void when_dispatching_an_event_with_single_registered_handler_then_invokes_handler()
        {
            var @event = new EventB();

            _sut.DispatchMessage(@event, "message", "correlation", "");

            _handler1Mock.As<IEventHandler<EventB>>().Verify(h => h.Handle(@event), Times.Once());
        }

        [Fact]
        public void when_dispatching_an_event_with_no_registered_handler_then_does_nothing()
        {
            var @event = new EventC();

            _sut.DispatchMessage(@event, "message", "correlation", "");
        }
    }

    public class EventA : IEvent
    {
        public Guid SourceId
        {
            get { return Guid.Empty; }
        }
    }

    public class EventB : IEvent
    {
        public Guid SourceId
        {
            get { return Guid.Empty; }
        }
    }

    public class EventC : IEvent
    {
        public Guid SourceId
        {
            get { return Guid.Empty; }
        }
    }
}
