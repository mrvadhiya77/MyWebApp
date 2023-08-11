using Microsoft.AspNetCore.Mvc;
using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitWork;

        public CategoryController(IUnitOfWork unitWork)
        {
            _unitWork = unitWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _unitWork.Categories.GetAll();
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
            _unitWork.Categories.Add(category);
            _unitWork.Save();
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
            var category = _unitWork.Categories.GetT(x => x.Id == id);
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
                _unitWork.Categories.Update(category);
                _unitWork.Save();
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
            var category = _unitWork.Categories.GetT(x => x.Id == id);
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
            var category = _unitWork.Categories.GetT(x => x.Id == id); 
            if (category == null)
            {
                return NotFound();
            }
            _unitWork.Categories.Delete(category);
            _unitWork.Save();
            TempData["success"] = "Category Deleted Successfully !";
            return RedirectToAction("Index");
        }
    }
}
