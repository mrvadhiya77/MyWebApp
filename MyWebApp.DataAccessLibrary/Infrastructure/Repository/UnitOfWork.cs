using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;

namespace MyWebApp.DataAccessLibrary.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }

        public ICartRepository Carts { get; private set; }

        public IApplicationUserRepository ApplicationUsers { get; private set; }

        private readonly MyWebAppContext _context;

        public UnitOfWork(MyWebAppContext context)
        {
            _context = context;
            Categories = new CategoryRepository(context);
            Products = new ProductRepository(context);
            Carts = new CartRepository(context);
            ApplicationUsers = new ApplicationUserRepository(context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
