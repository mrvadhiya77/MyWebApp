using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebApp.DataAccessLibrary.Infrastructure.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="orderDetail"></param>
        void Update(OrderDetail orderDetail);
    }
}
