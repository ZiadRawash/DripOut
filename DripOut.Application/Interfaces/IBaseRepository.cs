using DripOut.Application.DTOs;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
<<<<<<<< HEAD:DripOut.Application/Interfaces/ReposInterface/IBaseRepository.cs
namespace DripOut.Application.Interfaces.ReposInterface
========
namespace DripOut.Application.Interfaces
>>>>>>>> ece69bbb355b1077b50326df47f3e707701f1b32:DripOut.Application/Interfaces/IBaseRepository.cs
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<T> AddAsync(T entity);
<<<<<<<< HEAD:DripOut.Application/Interfaces/ReposInterface/IBaseRepository.cs
========


>>>>>>>> ece69bbb355b1077b50326df47f3e707701f1b32:DripOut.Application/Interfaces/IBaseRepository.cs
        public Task<T>? UpdateAsync(T entity);

        public Task<T> DeleteAsync(T entity);
        public  Task<T>? FindAsync(int id);
        public Task<T>? FindAsync(Expression<Func<T, bool>> expression ,
          params Expression<Func<T,object>>[] includes);
        public Task<IEnumerable<T>> GetAllAsync();

        public Task<IEnumerable<T>>? GetAllAsync(Expression<Func<T, bool>> expression,
          params Expression<Func<T, object>>[] includes);
        public IQueryable<T> GetAll(Expression<Func<T, bool>> expression);
    }
}
