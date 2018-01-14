using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using XSystem.Core.Domain;

namespace XSystem.Core.Infrastructure
{
    public class BaseEfRepository<T>:IRepository<T> where T : Model
    {
        private readonly DbSet<T> _dbSet;

        private readonly DbContext _context;

        public BaseEfRepository(DbContext context)
        {
            this._context = context;
            _dbSet = context.Set<T>();
        }

        public void Insert(T x)
        {
            _dbSet.Add(x);
        }

        public void Delete(T x)
        {
            if (_context.Entry(x).State==EntityState.Detached) {
                _dbSet.Attach(x);
            }
            _dbSet.Remove(x);
        }

        public void Delete(object id)
        {
            var find = _dbSet.Find(id);
            Delete(find);
        }

        public void Update(T x)
        {
            _dbSet.Attach(x);
            _context.Entry(x).State = EntityState.Modified;
        }

        public T GetById(Guid id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filterExpression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderByFunc = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;
            if (filterExpression != null) {
                query = query.Where(filterExpression);
            }
            query = includeProperties
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return orderByFunc?.Invoke(query).ToList() ?? query.ToList();
        }

        public IEnumerable<T> All()
        {
            return _dbSet;
        }

        public void Clear()
        {
            _dbSet.RemoveRange(this._dbSet);
        }
    }
}