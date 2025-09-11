using DripOut.Application.Interfaces.ReposInterface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DripOut.Persistence.Repositories
{
	public class BaseRepository<T> : IBaseRepository<T> where T : class
	{
		protected readonly ApplicationDbContext dbContext;
		protected readonly DbSet<T> dbSet;

		public BaseRepository(ApplicationDbContext dbContext)
		{
			this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			this.dbSet = dbContext.Set<T>();
		}

		public async Task<T?> AddAsync(T entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			await dbSet.AddAsync(entity);
//
			return entity;
		}

		public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
		{
			if (entities == null || !entities.Any())
				throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null or empty");

			await dbSet.AddRangeAsync(entities);
//
			return entities;
		}

		public async Task<T?> DeleteAsync(T entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			dbSet.Remove(entity);
//
			return entity;
		}

		public async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

		public async Task<T?> UpdateAsync(T entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			dbSet.Update(entity);

		 	return  entity;
		}

		public async Task<T?> FindAsync(int id)
		{
			if (id <= 0) return null;
			return await dbSet.FindAsync(id);
		}

		public async Task<T?> FindAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			var query = dbSet.AsQueryable().Where(expression);
			if (includes != null)
			{
				foreach (var include in includes)
					query = query.Include(include);
			}

			return await query.FirstOrDefaultAsync();
		}

		public async Task<T?> FindAsync(Expression<Func<T, bool>> expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			return await dbSet.FirstOrDefaultAsync(expression);
		}
		public async Task<T?> FindAsync(
			Expression<Func<T, bool>> expression,
			Func<IQueryable<T>, IQueryable<T>> include)
			{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			IQueryable<T> query = dbSet;

			if (include != null)
				query = include(query);

			return await query.FirstOrDefaultAsync(expression);
		}

		public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			var query = dbSet.Where(expression);

			if (includes != null)
			{
				foreach (var include in includes)
					query = query.Include(include);
			}

			return await query.ToListAsync();
		}

		public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes)
		{
			var query = dbSet.AsQueryable();

			if (includes != null)
			{
				foreach (var include in includes)
					query = query.Include(include);
			}

			return query;
		}
	}
}
