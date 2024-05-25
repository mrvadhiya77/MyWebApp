using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;

namespace MyWebApp.DataAccessLibrary.Infrastructure.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly MyWebAppContext _context;

        public ApplicationUserRepository(MyWebAppContext context) : base(context)
        {
            _context = context;
        }
    }
}
