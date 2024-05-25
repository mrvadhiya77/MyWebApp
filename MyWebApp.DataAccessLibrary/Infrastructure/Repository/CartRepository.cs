using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;

namespace MyWebApp.DataAccessLibrary.Infrastructure.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly MyWebAppContext _context;

        public CartRepository(MyWebAppContext context) : base(context)
        {
            _context = context;
        }
    }
}
