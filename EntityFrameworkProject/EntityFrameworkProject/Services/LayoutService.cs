using EntityFrameworkProject.Data;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkProject.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        public LayoutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, string>> GetSettings()
        {

            Dictionary<string, string> setting = _context.Settings.Where(m => !m.IsDeleted).AsEnumerable().ToDictionary(m => m.Key, m => m.Value);
            return setting;
        }
    }
}
