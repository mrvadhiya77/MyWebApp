using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebApp.DataAccessLibrary.Infrastructure.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        ICartRepository Carts { get; }
        IApplicationUserRepository ApplicationUsers { get; }
        IOrderHeaderRepository OrderHeaders { get; }
        IOrderDetailRepository OrderDetails { get; }

        void Save();
    }
}
