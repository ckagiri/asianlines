using System.Linq;
using System.Linq.Expressions;
using Ligi.Core.DataAccess;
using Ligi.Core.DomainBase;
using Ligi.Core.Messaging;
using System;
using System.Data.Entity;

namespace Ligi.Infrastructure.Sql.Database
{
    public class SqlDataContext<T> : IDataContext<T> where T : class, IAggregateRoot
    {
        private readonly IEventBus _eventBus;
        private readonly DbContext _context;

        public SqlDataContext(Func<DbContext> contextFactory, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _context = contextFactory.Invoke();
        }

        public T Find(Guid id)
        {
            return _context.Set<T>().Find(id);
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        public void Save(T aggregateRoot)
        {
            var entry = _context.Entry(aggregateRoot);

            if (entry.State == EntityState.Detached)
                _context.Set<T>().Add(aggregateRoot);

            // Can't have transactions across storage and message bus.
            _context.SaveChanges();

            var eventPublisher = aggregateRoot as IEventPublisher;
            if (eventPublisher != null)
                _eventBus.Publish(eventPublisher.Events);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SqlDataContext()
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
