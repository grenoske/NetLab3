using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true);
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, int page = 1, int amount = 5, string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
