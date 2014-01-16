using System;
using System.Diagnostics;
using System.IO;
using Ligi.Core;
using Ligi.Infrastructure.Sql.Serialization;

namespace Ligi.Infrastructure.Sql.Messaging.Handling
{
    public abstract class MessageProcessor : IProcessor, IDisposable
    {
        private readonly IMessageReceiver _receiver;
        private readonly ITextSerializer _serializer;
        private readonly object _lockObject = new object();
        private bool _disposed;
        private bool _started = false;

        protected MessageProcessor(IMessageReceiver receiver, ITextSerializer serializer)
        {
            _receiver = receiver;
            _serializer = serializer;
        }

        public virtual void Start()
        {
            ThrowIfDisposed();
            lock (_lockObject)
            {
                if (!_started)
                {
                    _receiver.MessageReceived += OnMessageReceived;
                    _receiver.Start();
                    _started = true;
                }
            }
        }

        public virtual void Stop()
        {
            lock (_lockObject)
            {
                if (_started)
                {
                    _receiver.Stop();
                    _receiver.MessageReceived -= OnMessageReceived;
                    _started = false;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void ProcessMessage(object payload, string correlationId);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Stop();
                    _disposed = true;

                    using (_receiver as IDisposable)
                    {
                        // Dispose receiver if it's disposable.
                    }
                }
            }
        }

        ~MessageProcessor()
        {
            Dispose(false);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Trace.WriteLine(new string('-', 100));

            try
            {
                var body = Deserialize(args.Message.Body);

                TracePayload(body);
                Trace.WriteLine("");

                ProcessMessage(body, args.Message.CorrelationId);

                Trace.WriteLine(new string('-', 100));
            }
            catch (Exception e)
            {
                Trace.TraceError("An exception happened while processing message through handler/s:\r\n{0}", e);
                Trace.TraceWarning("Error will be ignored and message receiving will continue.");
            }
        }

        protected object Deserialize(string serializedPayload)
        {
            using (var reader = new StringReader(serializedPayload))
            {
                return _serializer.Deserialize(reader);
            }
        }

        protected string Serialize(object payload)
        {
            using (var writer = new StringWriter())
            {
                _serializer.Serialize(writer, payload);
                return writer.ToString();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("MessageProcessor");
        }


        [Conditional("TRACE")]
        private void TracePayload(object payload)
        {
            Trace.WriteLine(Serialize(payload));
        }
    }
}
