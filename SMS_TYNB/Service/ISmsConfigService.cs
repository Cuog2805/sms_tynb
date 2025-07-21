using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Service
{
    public interface ISmsConfigService
    {
        Task Delete(SmsConfig obj);
        Task<SmsConfig?> Update(SmsConfig obj);
        Task<SmsConfig> Create(SmsConfig obj);
        Task<SmsConfig?> GetById(int id);
        Task<List<SmsConfig>> GetAll();
    }
}
