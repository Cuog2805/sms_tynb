using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.Repository
{
    public class ConfigRepository : BaseRepository<Config, long>
    {
        public ConfigRepository(VnptSmsBrandnameContext _context) : base(_context)
        {
        }
    }
}
