using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;

namespace MyWebApp.DataAccessLibrary.Infrastructure.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly MyWebAppContext _context;

        public OrderDetailRepository(MyWebAppContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
        }
    }
}
