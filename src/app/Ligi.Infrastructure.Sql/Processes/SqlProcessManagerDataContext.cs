using Ligi.Core.Commands;
using Ligi.Core.Messaging;
using Ligi.Core.Processes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Ligi.Infrastructure.Sql.Serialization;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
using Microsoft.Practices.TransientFaultHandling;

namespace Ligi.Infrastructure.Sql.Processes
{
    public class SqlProcessManagerDataContext<T> : IProcessManagerDataContext<T> where T : class, IProcessManager
    {
        private readonly ICommandBus _commandBus;
        private readonly DbContext _context;
        private readonly ITextSerializer _serializer;
        private readonly RetryPolicy<SqlAzureTransientErrorDetectionStrategy> _retryPolicy;

        public SqlProcessManagerDataContext(Func<DbContext> contextFactory, ICommandBus commandBus, ITextSerializer serializer)
        {
            _commandBus = commandBus;
            _context = contextFactory.Invoke();
            _serializer = serializer;

            _retryPolicy = new RetryPolicy<SqlAzureTransientErrorDetectionStrategy>(new Incremental(3, TimeSpan.Zero, TimeSpan.FromSeconds(1)) { FastFirstRetry = true });
            _retryPolicy.Retrying += (s, e) => 
                Trace.TraceWarning("An error occurred in attempt number {1} to save the process manager state: {0}", e.LastException.Message, e.CurrentRetryCount);
        }

        public T Find(Guid id)
        {
            return Find(pm => pm.Id == id, true);
        }

        public T Find(Expression<Func<T, bool>> predicate, bool includeCompleted = false)
        {
            T pm = null;
            if (!includeCompleted)
            {
                // first try to get the non-completed, in case the table is indexed by Completed, or there is more
                // than one process manager that fulfills the predicate but only 1 is not completed.
                pm = _retryPolicy.ExecuteAction(() => 
                    _context.Set<T>().Where(predicate.And(x => x.Completed == false)).FirstOrDefault());
            }

            if (pm == null)
            {
                pm = _retryPolicy.ExecuteAction(() => 
                    _context.Set<T>().Where(predicate).FirstOrDefault());
            }

            if (pm != null)
            {
                // TODO: ideally this could be improved to avoid 2 roundtrips to the server.
                var undispatchedMessages = _context.Set<UndispatchedMessages>().Find(pm.Id);
                try
                {
                    DispatchMessages(undispatchedMessages);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // if another thread already dispatched the messages, ignore
                    Trace.TraceWarning("Concurrency exception while marking commands as dispatched for process manager with ID {0} in Find method.", pm.Id);
                    
                    _context.Entry(undispatchedMessages).Reload();

                    undispatchedMessages = _context.Set<UndispatchedMessages>().Find(pm.Id);

                    // undispatchedMessages should be null, as we do not have a rowguid to do optimistic locking, other than when the row is deleted.
                    // Nevertheless, we try dispatching just in case the DB schema is changed to provide optimistic locking.
                    DispatchMessages(undispatchedMessages);
                }

                if (!pm.Completed || includeCompleted)
                {
                    return pm;
                }
            }

            return null;
        }

        /// <summary>
        /// Saves the state of the process manager and publishes the commands in a resilient way.
        /// </summary>
        /// <param name="processManager">The instance to save.</param>
        /// <remarks>For explanation of the implementation details, see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see>.</remarks>
        public void Save(T processManager)
        {
            var entry = _context.Entry(processManager);

            if (entry.State == EntityState.Detached)
                _context.Set<T>().Add(processManager);

            var commands = processManager.Commands.ToList();
            UndispatchedMessages undispatched = null;
            if (commands.Count > 0)
            {
                // if there are pending commands to send, we store them as undispatched.
                undispatched = new UndispatchedMessages(processManager.Id)
                                   {
                                       Commands = _serializer.Serialize(commands)
                                   };
                _context.Set<UndispatchedMessages>().Add(undispatched);
            }

            try
            {
                _retryPolicy.ExecuteAction(() => _context.SaveChanges());
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new ConcurrencyException(e.Message, e);
            }

            try
            {
                DispatchMessages(undispatched, commands);
            }
            catch (DbUpdateConcurrencyException)
            {
                // if another thread already dispatched the messages, ignore
                Trace.TraceWarning("Ignoring concurrency exception while marking commands as dispatched for process manager with ID {0} in Save method.", processManager.Id);
            }
        }

        private void DispatchMessages(UndispatchedMessages undispatched, List<Envelope<ICommand>> deserializedCommands = null)
        {
            if (undispatched != null)
            {
                if (deserializedCommands == null)
                {
                    deserializedCommands = _serializer.Deserialize<IEnumerable<Envelope<ICommand>>>(undispatched.Commands).ToList();
                }

                var originalCommandsCount = deserializedCommands.Count;
                try
                {
                    while (deserializedCommands.Count > 0)
                    {
                        _commandBus.Send(deserializedCommands.First());
                        deserializedCommands.RemoveAt(0);
                    }
                }
                catch (Exception)
                {
                    // We catch a generic exception as we don't know what implementation of ICommandBus we might be using.
                    if (originalCommandsCount != deserializedCommands.Count)
                    {
                        // if we were able to send some commands, then updates the undispatched messages.
                        undispatched.Commands = _serializer.Serialize(deserializedCommands);
                        try
                        {
                            _context.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            // if another thread already dispatched the messages, ignore and surface original exception instead
                        }
                    }

                    throw;
                }

                // we remove all the undispatched messages for this process manager.
                _context.Set<UndispatchedMessages>().Remove(undispatched);
                _retryPolicy.ExecuteAction(() => _context.SaveChanges());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SqlProcessManagerDataContext()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
