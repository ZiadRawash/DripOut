using DripOut.Application.DTOs;
using DripOut.Application.Interfaces;

using DripOut.Application.Interfaces.ReposInterface;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DripOut.Persistence.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected ApplicationDbContext dbContext { get; private set; }
        protected DbSet<T> dbSet { get; private set; }
        public BaseRepository(ApplicationDbContext dbcontext)
        {
            this.dbContext = dbcontext;
            this.dbSet = dbContext.Set<T>();
        }

        public async Task<T?> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }
        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();
            return entities;
        }

        public async Task<T?> DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

        public async Task<T?> UpdateAsync(T entity)
        {
            if (entity != null)
            {
                dbSet.Update(entity);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
            }
            return entity;
        }
        public async Task<T?> FindAsync(int id) => await dbSet.FindAsync(id)??null!;

        public async Task<T?> FindAsync(Expression<Func<T, bool>> expression
            ,params Expression<Func<T, object>>[] includes)
        {
            var query = dbSet.Where(expression);
            if (query != null && includes != null)
            {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
            }
            return await query!.FirstAsync();
		}

        public async Task<IEnumerable<T>?> GetAllAsync(Expression<Func<T, bool>> expression,
           params Expression<Func<T, object>>[] includes)
        {
            var query = dbSet.Where(expression);
            foreach(var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }
        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            var query = dbSet.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }
    }
}
