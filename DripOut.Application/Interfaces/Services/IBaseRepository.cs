using DripOut.Application.DTOs;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
namespace DripOut.Application.Interfaces.Services
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<T> AddAsync(T entity);

        public  Task<T>? FindAsync(int id);

        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T>? UpdateAsync(T entity);

        public Task<T> DeleteAsync(T entity);
        public Task<T>? FindAsync(Expression<Func<T, bool>> expression ,
          params Expression<Func<T,object>>[] includes);

        public Task<IEnumerable<T>>? FindAllAsync(Expression<Func<T,bool>> expression,
          params Expression<Func<T,object>>[] includes);
    }
}
