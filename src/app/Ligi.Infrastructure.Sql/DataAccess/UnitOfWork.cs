using System.Data.Entity;
using Ligi.Core.DataAccess;

namespace Ligi.Infrastructure.Sql.DataAccess
{
    public abstract class UnitOfWork<TContext> : IUnitOfWork
      where TContext : DbContext, IContext, new()
    {
        private readonly DbContext _context;

        protected UnitOfWork()
        {
            _context = new TContext();

            // Do NOT enable proxied entities, else serialization fails
            _context.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            _context.Configuration.LazyLoadingEnabled = false;
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public IContext Context
        {
            get { return (TContext) _context; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
