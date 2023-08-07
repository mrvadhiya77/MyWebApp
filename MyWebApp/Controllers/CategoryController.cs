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
                TempData["success"] = "Category Added Successfully !";
            return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }
            var category = _appContext.Categories.Find(id);
            if(category==null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _appContext.Categories.Update(category);
                _appContext.SaveChanges();
                TempData["success"] = "Category Edited Successfully !";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DeleteCategory(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _appContext.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteData(int? id)
        {
            var category = _appContext.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            _appContext.Categories.Remove(category);
            _appContext.SaveChanges();
            TempData["success"] = "Category Deleted Successfully !";
            return RedirectToAction("Index");
        }
    }
}
