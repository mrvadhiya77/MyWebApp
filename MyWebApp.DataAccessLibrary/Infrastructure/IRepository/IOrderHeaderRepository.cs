using MyWebApp.Models;

namespace MyWebApp.DataAccessLibrary.Infrastructure.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="orderHeader"></param>
        void Update(OrderHeader orderHeader);

        /// <summary>
        /// Update Payment Status And Order Status
        /// </summary>
        void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null);

        /// <summary>
        /// Payment Status
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="SessionId"></param>
        /// <param name="PaymentIntentId"></param>
        void PaymentStatus(int Id, string SessionId, string PaymentIntentId);
    }
}
