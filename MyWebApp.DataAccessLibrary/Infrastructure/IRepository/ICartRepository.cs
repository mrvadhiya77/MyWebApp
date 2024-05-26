using MyWebApp.Models;

namespace MyWebApp.DataAccessLibrary.Infrastructure.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        int IncrementCartItem(Cart cart, int Count);
        int DecrementCartItem(Cart cart, int Count);
    }
}
