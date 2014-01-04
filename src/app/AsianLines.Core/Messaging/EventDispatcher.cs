using AsianLines.Core.Events;
using AsianLines.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ligi.Core.Messaging
{
    public class EventDispatcher
    {
        private readonly Dictionary<Type, List<Tuple<Type, Action<Envelope>>>> _handlersByEventType;
        private readonly Dictionary<Type, Action<IEvent, string, string, string>> _dispatchersByEventType;

        public EventDispatcher()
        {
            _handlersByEventType = new Dictionary<Type, List<Tuple<Type, Action<Envelope>>>>();
            _dispatchersByEventType = new Dictionary<Type, Action<IEvent, string, string, string>>();
        }

        public EventDispatcher(IEnumerable<IEventHandler> handlers)
            : this()
        {
            foreach (var handler in handlers)
            {
                Register(handler);
            }
        }

        public void Register(IEventHandler handler)
        {
            var handlerType = handler.GetType();

            foreach (var invocationTuple in BuildHandlerInvocations(handler))
            {
                var envelopeType = typeof(Envelope<>).MakeGenericType(invocationTuple.Item1);

                List<Tuple<Type, Action<Envelope>>> invocations;
                if (!_handlersByEventType.TryGetValue(invocationTuple.Item1, out invocations))
                {
                    invocations = new List<Tuple<Type, Action<Envelope>>>();
                    _handlersByEventType[invocationTuple.Item1] = invocations;
                }
                invocations.Add(new Tuple<Type, Action<Envelope>>(handlerType, invocationTuple.Item2));

                if (!_dispatchersByEventType.ContainsKey(invocationTuple.Item1))
                {
                    _dispatchersByEventType[invocationTuple.Item1] = BuildDispatchInvocation(invocationTuple.Item1);
                }
            }
        }

        public void DispatchMessages(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                DispatchMessage(@event);
            }
        }

        public void DispatchMessage(IEvent @event)
        {
            DispatchMessage(@event, null, null, "");
        }

        public void DispatchMessage(IEvent @event, string messageId, string correlationId, string traceIdentifier)
        {
            Action<IEvent, string, string, string> dispatch;
            if (_dispatchersByEventType.TryGetValue(@event.GetType(), out dispatch))
            {
                dispatch(@event, messageId, correlationId, traceIdentifier);
            }
            // Invoke also the generic handlers that have registered to handle IEvent directly.
            if (_dispatchersByEventType.TryGetValue(typeof(IEvent), out dispatch))
            {
                dispatch(@event, messageId, correlationId, traceIdentifier);
            }
        }

        private void DoDispatchMessage<T>(T @event, string messageId, string correlationId, string traceIdentifier)
            where T : IEvent
        {
            var envelope = Envelope.Create(@event);
            envelope.MessageId = messageId;
            envelope.CorrelationId = correlationId;

            List<Tuple<Type, Action<Envelope>>> handlers;
            if (_handlersByEventType.TryGetValue(typeof(T), out handlers))
            {
                foreach (var handler in handlers)
                {
                    // Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Event{0} handled by {1}.", traceIdentifier, handler.Item1.FullName));
                    handler.Item2(envelope);
                }
            }
        }

        private IEnumerable<Tuple<Type, Action<Envelope>>> BuildHandlerInvocations(IEventHandler handler)
        {
            var interfaces = handler.GetType().GetInterfaces();

            var eventHandlerInvocations =
                interfaces
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    .Select(i => new { HandlerInterface = i, EventType = i.GetGenericArguments()[0] })
                    .Select(e => new Tuple<Type, Action<Envelope>>(e.EventType, BuildHandlerInvocation(handler, e.HandlerInterface, e.EventType)));

            var envelopedEventHandlerInvocations =
                interfaces
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnvelopedEventHandler<>))
                    .Select(i => new { HandlerInterface = i, EventType = i.GetGenericArguments()[0] })
                    .Select(e => new Tuple<Type, Action<Envelope>>(e.EventType, BuildEnvelopeHandlerInvocation(handler, e.HandlerInterface, e.EventType)));

            return eventHandlerInvocations.Union(envelopedEventHandlerInvocations);
        }

        private Action<Envelope> BuildHandlerInvocation(IEventHandler handler, Type handlerType, Type messageType)
        {
            var envelopeType = typeof(Envelope<>).MakeGenericType(messageType);

            var parameter = Expression.Parameter(typeof(Envelope));
            var invocationExpression =
                Expression.Lambda(
                    Expression.Block(
                        Expression.Call(
                            Expression.Convert(Expression.Constant(handler), handlerType),
                            handlerType.GetMethod("Handle"),
                            Expression.Property(Expression.Convert(parameter, envelopeType), "Body"))),
                    parameter);

            return (Action<Envelope>)invocationExpression.Compile();
        }

        private Action<Envelope> BuildEnvelopeHandlerInvocation(IEventHandler handler, Type handlerType, Type messageType)
        {
            var envelopeType = typeof(Envelope<>).MakeGenericType(messageType);

            var parameter = Expression.Parameter(typeof(Envelope));
            var invocationExpression =
                Expression.Lambda(
                    Expression.Block(
                        Expression.Call(
                            Expression.Convert(Expression.Constant(handler), handlerType),
                            handlerType.GetMethod("Handle"),
                            Expression.Convert(parameter, envelopeType))),
                    parameter);

            return (Action<Envelope>)invocationExpression.Compile();
        }

        private Action<IEvent, string, string, string> BuildDispatchInvocation(Type eventType)
        {
            var eventParameter = Expression.Parameter(typeof(IEvent));
            var messageIdParameter = Expression.Parameter(typeof(string));
            var correlationIdParameter = Expression.Parameter(typeof(string));
            var traceIdParameter = Expression.Parameter(typeof(string));

            var dispatchExpression =
                Expression.Lambda(
                    Expression.Block(
                        Expression.Call(
                            Expression.Constant(this),
                            this.GetType().GetMethod("DoDispatchMessage", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(eventType),
                            Expression.Convert(eventParameter, eventType),
                            messageIdParameter,
                            correlationIdParameter,
                            traceIdParameter)),
                    eventParameter,
                    messageIdParameter,
                    correlationIdParameter,
                    traceIdParameter);

            return (Action<IEvent, string, string, string>)dispatchExpression.Compile();
        }
    }
}