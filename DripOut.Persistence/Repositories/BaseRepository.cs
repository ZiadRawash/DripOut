using DripOut.Application.DTOs;
using DripOut.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DripOut.Persistence.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public ApplicationDbContext dbContext { get; private set; }
        public DbSet<T> dbSet { get; private set; }
        public BaseRepository(ApplicationDbContext dbcontext)
        {
            this.dbContext = dbcontext;
            this.dbSet = dbContext.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

        public async Task<T>? FindAsync(int id) => await dbSet.FindAsync(id);


        public async Task<T>? UpdateAsync(T entity)
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

        public async Task<T>? FindAsync(Expression<Func<T, bool>> expression
            ,params Expression<Func<T, object>>[] includes)
        {
            var query = dbSet.Where(expression);
            foreach(var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>>? GetAllAsync(Expression<Func<T, bool>> expression,
           params Expression<Func<T, object>>[] includes)
        {
            var query = dbSet.Where(expression);
            foreach(var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public  IQueryable<T> GetAll(Expression<Func<T, bool>> expression)
            =>  dbSet.Where(expression);
    }
}
