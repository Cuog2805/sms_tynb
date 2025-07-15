using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Common;
using SMS_TYNB.Models.Master;
using System.Linq.Dynamic.Core;

namespace TodoApi.Repository
{
	public class BaseRepository<T, TKey> where T : class where TKey : notnull
	{
		protected readonly SmsTynContext context;

		public BaseRepository(SmsTynContext _context)
		{
			context = _context;
		}

		public virtual async Task<IEnumerable<T>> GetAll()
		{
			return await context.Set<T>().ToListAsync();
		}

		public async Task<IEnumerable<T>> GetPagination(IQueryable<T> query, Pageable pageable)
		{
			//sắp xếp động
			if (!string.IsNullOrEmpty(pageable.Sort))
			{
				query = query.OrderBy(pageable.Sort);
			}

			return await query
				.Skip((pageable.PageNumber - 1) * pageable.PageSize)
				.Take(pageable.PageSize)
				.ToListAsync();
		}

		public async Task<T?> FindById(TKey id)
		{
			return await context.Set<T>().FindAsync(id);
		}

		public async Task<T> Create(T entity)
		{
			var entityNew = await context.Set<T>().AddAsync(entity);
			await context.SaveChangesAsync();

			return entityNew.Entity;
		}

		public async Task<T?> Update(TKey id, T entityUpdate)
		{
			var existingEntity = await context.Set<T>().FindAsync(id);
			if (existingEntity == null)
				return null;

			context.Entry(existingEntity).CurrentValues.SetValues(entityUpdate);
			await context.SaveChangesAsync();

			return existingEntity;
		}

		public async Task Delete(TKey id)
		{
			T? entity = await FindById(id);

			if(entity != null)
			{
				context.Set<T>().Remove(entity);
				await context.SaveChangesAsync();
			}
		}
	}
}
