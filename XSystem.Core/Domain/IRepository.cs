using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace XSystem.Core.Domain
{
    public interface IRepository<T> where T:Model
    {
        void Insert(T x);
        void Delete(T x);
        void Delete(object id);
        void Update(T x);
        T GetById(Guid id);
        IEnumerable<T> Get(Expression<Func<T, bool>> filterExpression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderByFunc = null,
            string includeProperties = "");
        IEnumerable<T> All();
        /// <summary>
        /// 清空指定实体对应的表
        /// </summary>
        void Clear();
    }
}