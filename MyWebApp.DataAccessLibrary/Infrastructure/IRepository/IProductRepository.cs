using MyWebApp.Models;

namespace MyWebApp.DataAccessLibrary.Infrastructure.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="product"></param>
        void Update(Product product);
    }
}
