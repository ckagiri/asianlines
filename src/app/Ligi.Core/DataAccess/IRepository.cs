using System;
using System.Linq;
using System.Linq.Expressions;

namespace Ligi.Core.DataAccess
{
    public interface IRepository<T> where T : class
    {
        T Find(Guid id);
        T Find(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindAll(Expression<Func<T, bool>> predicate = null);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Guid id);

        T FirstOrDefault(Expression<Func<T, bool>> predicate = null);
        int Count(Expression<Func<T, bool>> predicate = null);
        bool Exists(Expression<Func<T, bool>> predicate = null);
    }
}
