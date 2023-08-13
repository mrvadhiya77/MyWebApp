using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebApp.DataAccessLibrary.Infrastructure.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="category"></param>
        void Update(Category category);
    }
}
