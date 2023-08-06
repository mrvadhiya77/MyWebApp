using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class CategoryController : Controller
    {
        private MyWebAppContext _appContext;

        public CategoryController(MyWebAppContext appContext)
        {
            _appContext = appContext;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _appContext.Categories;
            return View(categories);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(Category category)
        {
            if(ModelState.IsValid)
            {
            _appContext.Categories.Add(category);
            _appContext.SaveChanges();
            return RedirectToAction("Index");
            }
            return View(category);
        }
    }
}
