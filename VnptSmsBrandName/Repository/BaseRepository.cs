using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Models.Master;
using System.Linq.Dynamic.Core;

namespace VnptSmsBrandName.Repository
{
	public class BaseRepository<T, TKey> where T : class where TKey : notnull
	{
		protected readonly VnptSmsBrandnameContext context;

		public BaseRepository(VnptSmsBrandnameContext _context)
		{
			context = _context;
		}

		public virtual async Task<IEnumerable<T>> GetAll()
		{
			return await context.Set<T>().ToListAsync();
		}

		public virtual async Task<IEnumerable<T>> GetPagination(IQueryable<T> query, Pageable pageable)
		{
			//s?p x?p d?ng
			if (!string.IsNullOrEmpty(pageable.Sort))
			{
				query = query.OrderBy(pageable.Sort);
			}

			return await query
				.Skip((pageable.PageNumber - 1) * pageable.PageSize)
				.Take(pageable.PageSize)
				.ToListAsync();
		}

		public virtual async Task<T?> FindById(TKey id)
		{
			return await context.Set<T>().FindAsync(id);
		}

		public virtual async Task<T> Create(T entity)
		{
			var entityNew = await context.Set<T>().AddAsync(entity);
			await context.SaveChangesAsync();

			return entityNew.Entity;
		}

		public virtual async Task<List<T>> CreateRange(List<T> entities)
		{
			context.Set<T>().AddRange(entities);
			await context.SaveChangesAsync();
			return entities;
		}

		public virtual async Task<T?> Update(TKey id, T entityUpdate)
		{
			var existingEntity = await context.Set<T>().FindAsync(id);
			if (existingEntity == null)
				return null;

			context.Entry(existingEntity).CurrentValues.SetValues(entityUpdate);
			await context.SaveChangesAsync();

			return existingEntity;
		}
        public IQueryable<T> Query()
        {
            return context.Set<T>().AsQueryable();
        }

		public IQueryable<T> GetAllByOrgId(long orgId)
		{
			return context.Set<T>().Where(e => EF.Property<long>(e, "IdOrganization") == orgId);
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
