using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
    public class SmsConfigRepository : BaseRepository<SmsConfig, long>
    {
        public SmsConfigRepository(SmsTynContext _context) : base(_context)
        {
        }
    }
}
