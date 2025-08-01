using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.Repository
{
	public class MEmployeeRepository : BaseRepository<MEmployee, long>
	{
		public MEmployeeRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}

		public Task<IQueryable<MEmployee>> Search(string? searchInput, long orgId)
		{
			var query = context.Set<MEmployee>().AsQueryable().Where(item => item.OrganizationId == orgId);

			if (!string.IsNullOrWhiteSpace(searchInput))
			{
				var pattern = "%" + searchInput.Trim().ToLower() + "%";

				query = query.Where(item =>
					EF.Functions.Like(item.Name, pattern)
					|| EF.Functions.Like(item.PhoneNumber, pattern)
				);
			}

			return Task.FromResult(query);
		}

		/// <summary>
		/// tìm ki?m MEmployee có sdt t?n t?i danh sách sdt truy?n vào
		/// </summary>
		/// <param name="phonenumbers"></param>
		/// <returns></returns>
		public Task<IQueryable<MEmployee>> FindByPhoneNumbersAndIdOrganization(List<string> phonenumbers, long orgId)
		{
			var query = context.Set<MEmployee>().Where(item => phonenumbers.Contains(item.PhoneNumber) && item.OrganizationId == orgId);

			return Task.FromResult(query);
		}

		public override async Task<MEmployee> Create(MEmployee entity)
		{
			entity.PhoneNumberSend = !string.IsNullOrWhiteSpace(entity.PhoneNumber) && entity.PhoneNumber.StartsWith("0")
								? string.Concat("84", entity.PhoneNumber.Trim().AsSpan(1))
								: entity.PhoneNumber?.Trim() ?? "";
			var entityNew = await context.Set<MEmployee>().AddAsync(entity);
			await context.SaveChangesAsync();

			return entityNew.Entity;
		}

		public override async Task<List<MEmployee>> CreateRange(List<MEmployee> entities)
		{
			foreach (var entity in entities)
			{
				entity.PhoneNumberSend = !string.IsNullOrWhiteSpace(entity.PhoneNumber) && entity.PhoneNumber.StartsWith("0")
								? string.Concat("84", entity.PhoneNumber.Trim().AsSpan(1))
								: entity.PhoneNumber?.Trim() ?? "";
			}

			context.Set<MEmployee>().AddRange(entities);
			await context.SaveChangesAsync();
			return entities;
		}

		public override async Task<MEmployee?> Update(long id, MEmployee entityUpdate)
		{
			var existingEntity = await context.Set<MEmployee>().FindAsync(id);
			if (existingEntity == null)
				return null;

			entityUpdate.PhoneNumberSend = !string.IsNullOrWhiteSpace(entityUpdate.PhoneNumber) && entityUpdate.PhoneNumber.StartsWith("0")
								? string.Concat("84", entityUpdate.PhoneNumber.Trim().AsSpan(1))
								: entityUpdate.PhoneNumber?.Trim() ?? "";

			context.Entry(existingEntity).CurrentValues.SetValues(entityUpdate);
			await context.SaveChangesAsync();

			return existingEntity;
		}
	}
}
