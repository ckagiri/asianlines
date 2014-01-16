using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Ligi.Core.DataAccess;

namespace Ligi.Infrastructure.Sql.DataAccess
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private DbSet<TEntity> _dbSet;
        private readonly DbContext _dbContext;

        protected RepositoryBase(IUnitOfWork uow)
        {
                _dbContext = uow.Context as DbContext;
        }

        protected DbContext DbContext 
        {
            get { return _dbContext; }
        }

        protected virtual IQueryable<TEntity> Query
        {
            get
            {
                return DbSet;
            }
        }

        protected DbSet<TEntity> DbSet
        {
            get { return _dbSet ?? (_dbSet = DbContext.Set<TEntity>()); }
        }

        public virtual TEntity Find(Guid id)
        {
            return DbSet.Find(id);
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.SingleOrDefault(predicate);
        }

        public virtual IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null)
        {
            return (predicate == null)
                ? DbSet
                : DbSet.Where(predicate);
        }

        public virtual void Add(TEntity entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }

        public virtual void Update(TEntity entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        public virtual void Delete(Guid id)
        {
            var entity = Find(id);
            if (entity == null) return; 
            Delete(entity);
        }

        public virtual void Save()
        {
            DbContext.SaveChanges();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            return (predicate == null)
                ? DbSet.Count()
                : DbSet.Count(predicate);
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate = null)
        {
            return (predicate == null)
                ? DbSet.Any()
                : DbSet.Any(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null)
        {
            return (predicate == null)
                ? DbSet.FirstOrDefault()
                : DbSet.FirstOrDefault(predicate);
        }
    }
}
