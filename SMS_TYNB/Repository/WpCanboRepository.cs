using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpCanboRepository : BaseRepository<WpCanbo, int>
	{
		public WpCanboRepository(SmsTynContext _context) : base(_context)
		{
		}

		public Task<IQueryable<WpCanbo>> Search(string? searchInput)
		{
			var query = context.Set<WpCanbo>().AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchInput))
			{
				var pattern = "%" + searchInput.Trim().ToLower() + "%";

				query = query.Where(item =>
					EF.Functions.Like(item.TenCanbo, pattern)
					|| EF.Functions.Like(item.SoDt, pattern)
				);
			}

			return Task.FromResult(query);
		}

		public override async Task<WpCanbo> Create(WpCanbo entity)
		{
			entity.SoDTGui = !string.IsNullOrWhiteSpace(entity.SoDt) && entity.SoDt.StartsWith("0")
								? string.Concat("84", entity.SoDt.Trim().AsSpan(1))
								: entity.SoDt?.Trim() ?? "";
			var entityNew = await context.Set<WpCanbo>().AddAsync(entity);
			await context.SaveChangesAsync();

			return entityNew.Entity;
		}

		public override async Task<WpCanbo?> Update(int id, WpCanbo entityUpdate)
		{
			var existingEntity = await context.Set<WpCanbo>().FindAsync(id);
			if (existingEntity == null)
				return null;

			existingEntity.SoDTGui = !string.IsNullOrWhiteSpace(existingEntity.SoDt) && existingEntity.SoDt.StartsWith("0")
								? string.Concat("84", existingEntity.SoDt.Trim().AsSpan(1))
								: existingEntity.SoDt?.Trim() ?? "";

			context.Entry(existingEntity).CurrentValues.SetValues(entityUpdate);
			await context.SaveChangesAsync();

			return existingEntity;
		}
	}
}
