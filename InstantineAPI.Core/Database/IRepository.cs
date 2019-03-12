using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InstantineAPI.Data;

namespace InstantineAPI.Core.Database
{
    public interface IRepository<T> where T : Entity
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression);
        Task<T> GetFirst(Expression<Func<T, bool>> expression);
        Task Delete(T element);
        Task Delete(Expression<Func<T, bool>> expression);
        Task Update(T entity);
        Task Add(T entity);
        Task SaveChanges();
        Task<bool> Any(Expression<Func<T, bool>> predicate);
    }
}
