using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;

namespace MyWebApp.DataAccessLibrary.Infrastructure.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly MyWebAppContext _context;

        public OrderHeaderRepository(MyWebAppContext context) : base(context) 
        {
            _context = context;
        }

        /// <summary>
        /// Add Payment Status
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="SessionId"></param>
        /// <param name="PaymentIntentId"></param>
        public void PaymentStatus(int Id, string SessionId, string PaymentIntentId)
        {
            var orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == Id);
            orderHeader.PaymentIntentId = PaymentIntentId;
            orderHeader.SessionId = SessionId;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);  
        }

        public void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null)
        {
            var order = _context.OrderHeaders.FirstOrDefault(x => x.Id == Id);
            if(order != null)
            {
                order.OrderStatus = orderStatus;
            }
            if (paymentStatus != null)
            {
                order.PaymentStatus = paymentStatus;
            }
        }
    }
}
