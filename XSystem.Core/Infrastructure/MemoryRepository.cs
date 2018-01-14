using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using XSystem.Core.Domain;

namespace XSystem.Core.Infrastructure
{
    /// <summary>
    /// 测试用例
    /// </summary>
    public class MemoryRepository<T>:IRepository<T> where T:Model
    {
        private readonly Dictionary<Guid, T> _dictionary = new Dictionary<Guid, T>();
        
        public void Insert(T x)
        {
            x.Id = Guid.NewGuid();
            _dictionary.Add(x.Id,x);
        }

        public void Delete(T x)
        {
            _dictionary.Remove(x.Id);
        }

        public void Delete(object id)
        {
            _dictionary.Remove((Guid) id);
        }

        public void Update(T x)
        {
            //do nothing
        }

        public T GetById(Guid id)
        {
            if (_dictionary.TryGetValue(id,out T value)) {
                return value;
            }
            throw new Exception("this id haven't saved");
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filterExpression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderByFunc = null,
            string includeProperties = "")
        {
            var query = _dictionary.Values.AsQueryable();
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
            return _dictionary.Values;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }
    }
}