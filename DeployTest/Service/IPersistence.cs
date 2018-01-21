using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DeployTest.Service
{
    public interface IPersistence
    {
        IQueryable<T> Get<T>(Expression<Func<T, bool>> filterException = null) where T : class;
        void Save(VisionReptile reptile);
    }
}