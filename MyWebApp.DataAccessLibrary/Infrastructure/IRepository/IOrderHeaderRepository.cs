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
    }
}
