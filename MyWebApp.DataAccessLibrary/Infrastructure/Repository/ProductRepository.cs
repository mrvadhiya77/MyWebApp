using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;
namespace MyWebApp.DataAccessLibrary.Infrastructure.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly MyWebAppContext _context;

        public ProductRepository(MyWebAppContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productDB = _context.Products.FirstOrDefault(x => x.Id == product.Id);

            if (productDB != null)
            {
                productDB.Name = product.Name;
                productDB.Description = product.Description;
                productDB.Price = product.Price;
                if(productDB.ImageUrl != null)
                {
                    productDB.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
