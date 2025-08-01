using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Repository;
using VnptSmsBrandName.ViewModel;
using System.Threading.Tasks;
using VnptSmsBrandName.Helper;
using VnptSmsBrandName.Models.Identity;

namespace VnptSmsBrandName.Service
{
    public class SmsConfigService : ISmsConfigService
    {
        private readonly SmsConfigRepository _smsConfigRepository;

        public SmsConfigService(SmsConfigRepository smsConfigRepository)
        {
            _smsConfigRepository = smsConfigRepository;
        }

        public async Task<PageResult<SmsConfig>> SearchSmsConfig(SmsConfigSearchViewModel model, Pageable pageable, long orgId)
        {
            IQueryable<SmsConfig> smsConfigs = await _smsConfigRepository.Search(model, orgId);
            var wpCanbosPage = await _smsConfigRepository.GetPagination(smsConfigs, pageable);

            int total = await smsConfigs.CountAsync();

            return new PageResult<SmsConfig>
            {
                Data = wpCanbosPage,
                Total = total,
            };
        }

		public List<SmsConfig> GetAll(long orgId)
		{
			var data = _smsConfigRepository.GetAllByOrgId(orgId);
			return data.ToList();
		}

		public SmsConfig GetSmsConfigByOrgIdAndActive(long orgId)
		{
			var data = _smsConfigRepository.GetSmsConfigByOrgIdAndActive(orgId) ?? new SmsConfig();
			return data;
		}

		public async Task<SmsConfig?> GetByIdAndOrgId(long id, long orgId)
        {
            SmsConfig? data = await _smsConfigRepository.FindByIdAndOrgId(id, orgId);
            return data;
        }

        public async Task<SmsConfig> Create(SmsConfig obj, Users user)
        {
            AuditHelper.SetCreateAudit(obj, user);
            SmsConfig newObj = await _smsConfigRepository.Create(obj);
            return obj;
        }

        public async Task<SmsConfig?> Update(SmsConfig obj, Users user)
        {
            AuditHelper.SetUpdateAudit(obj, user);
            SmsConfig? newObj = await _smsConfigRepository.Update(obj.Id, obj);
            return newObj;
        }
    }

	public interface ISmsConfigService
	{
		Task<SmsConfig?> Update(SmsConfig obj, Users user);
		Task<SmsConfig> Create(SmsConfig obj, Users user);
		Task<SmsConfig?> GetByIdAndOrgId(long id, long orgId);
		List<SmsConfig> GetAll(long orgId);
        SmsConfig GetSmsConfigByOrgIdAndActive(long orgId);
		Task<PageResult<SmsConfig>> SearchSmsConfig(SmsConfigSearchViewModel model, Pageable pageable, long orgId);
	}
}
