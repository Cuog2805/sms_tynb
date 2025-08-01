using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Repository;
using System.Threading.Tasks;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Helper;

namespace VnptSmsBrandName.Service
{
    public class ConfigService : IConfigService
    {
        private readonly ConfigRepository _configRepository;
        public ConfigService(ConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }
        public async Task<List<Config>> GetAllByOrgId(long orgId)
        {
            var data = _configRepository.GetAllByOrgId(orgId);
            return data.ToList();
        }
        public async Task<Config> Create(Config obj, Users user)
        {
            AuditHelper.SetCreateAudit(obj, user);
			Config newObj = await _configRepository.Create(obj);
            return obj;
        }
        public async Task<Config?> Update(Config obj, Users user)
        {
			AuditHelper.SetUpdateAudit(obj, user);
			Config? newObj = await _configRepository.Update(obj.Id, obj);
            return newObj;
        }
		public async Task<Config?> FindByKey(string key)
		{
			var result = await _configRepository.Query().Where(t => t.Key == key).FirstOrDefaultAsync();
			return result;
		}

		public async Task<Config?> FindByKeyAndOrgId(string key, long orgId)
        {
            var result = await _configRepository.Query().Where(t => t.Key == key && t.OrganizationId == orgId).FirstOrDefaultAsync();
            return result;
        }
    }

	public interface IConfigService
	{
		Task<Config?> Update(Config obj, Users user);
		Task<Config> Create(Config obj, Users user);
		Task<List<Config>> GetAllByOrgId(long orgId);
		Task<Config?> FindByKey(string key);
		Task<Config?> FindByKeyAndOrgId(string key, long orgId);
	}
}
