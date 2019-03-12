using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InstantineAPI.Core.Database;
using InstantineAPI.Data;

namespace InstantineAPI.Database.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : Entity
    {
        protected InstantineDbContext InstantineDbContext { get; }
        protected BaseRepository(InstantineDbContext instantineDbContext)
        {
            InstantineDbContext = instantineDbContext;
        }

        protected abstract IQueryable<T> Query { get; }
        protected abstract DbSet<T> DbSet { get; }

        public virtual Task Delete(T element)
        {
            InstantineDbContext.Remove(element);
            return SaveChanges();
        }

        public virtual async Task Delete(Expression<Func<T, bool>> expression)
        {
            var elements = await GetAll(expression);
            foreach (var element in elements)
            {
                InstantineDbContext.Remove(element);
            }
            await SaveChanges();
        }

        public virtual Task<T> GetFirst(Expression<Func<T, bool>> expression)
        {
            return Query.FirstOrDefaultAsync(expression);
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await Query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression)
        {
            return await Query.Where(expression).ToListAsync();
        }
        public virtual Task SaveChanges()
        {
            return InstantineDbContext.SaveChangesAsync();
        }

        public virtual Task Update(T entity)
        {
            InstantineDbContext.Update(entity);
            return SaveChanges();
        }

        public async Task Add(T entity)
        {
            await DbSet.AddAsync(entity);
            await SaveChanges();
        }

        public Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            return Query.AnyAsync(predicate);
        }
    }
}
