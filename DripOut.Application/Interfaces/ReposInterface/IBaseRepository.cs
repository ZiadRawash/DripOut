using DripOut.Application.DTOs;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DripOut.Application.Interfaces.ReposInterface
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<T?> AddAsync(T entity);
        public Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        public Task<T?> UpdateAsync(T entity);

        public Task<T?> DeleteAsync(T entity);
        public  Task<T?> FindAsync(int id);
        public Task<T?> FindAsync(Expression<Func<T, bool>> expression ,
          params Expression<Func<T,object>>[] includes);
        public Task<IEnumerable<T>> GetAllAsync();

        public Task<IEnumerable<T>?> GetAllAsync(Expression<Func<T, bool>> expression,
          params Expression<Func<T, object>>[] includes);
        public IQueryable<T>? GetAll(Expression<Func<T, bool>> expression);
    }
}
