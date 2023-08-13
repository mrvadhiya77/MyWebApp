using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebApp.DataAccessLibrary.Infrastructure.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly MyWebAppContext _context;

        public CategoryRepository(MyWebAppContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var categoryDB = _context.Categories.FirstOrDefault(x => x.Id == category.Id);  
            
            if (categoryDB != null)
            {
                categoryDB.Name = category.Name;
                categoryDB.DisplayOrder = category.DisplayOrder;
            }
        }
    }
}
