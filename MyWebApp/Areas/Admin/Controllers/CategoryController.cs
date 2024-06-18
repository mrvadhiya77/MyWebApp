using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.CommonHelperRole;
using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;
using MyWebApp.Models.ViewModels;

namespace MyWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebsiteRole.Role_Admin)]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitWork;

        public CategoryController(IUnitOfWork unitWork)
        {
            _unitWork = unitWork;
        }

        public IActionResult Index()
        {
            CategoryVM categoryVM = new CategoryVM();
            categoryVM.categories = _unitWork.Categories.GetAll();
            return View(categoryVM);
        }

        #region Add Category
        //[HttpGet]
        //public IActionResult AddCategory()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AddCategory(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitWork.Categories.Add(category);
        //        _unitWork.Save();
        //        TempData["success"] = "Category Added Successfully !";
        //        return RedirectToAction("Index");
        //    }
        //    return View(category);
        //}
        #endregion

        #region Edit Category
        //[HttpGet]
        //public IActionResult EditCategory(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var category = _unitWork.Categories.GetT(x => x.Id == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(category);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult EditCategory(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitWork.Categories.Update(category);
        //        _unitWork.Save();
        //        TempData["success"] = "Category Edited Successfully !";
        //        return RedirectToAction("Index");
        //    }
        //    return RedirectToAction("Index");
        //}
        #endregion

        #region Add Edit Category
        [HttpGet]
        public IActionResult AddEditCategory(int? id)
        {
            CategoryVM categoryVM = new CategoryVM();
            if (id == null || id == 0)
            {
                return View(categoryVM);
            }
            else
            {
                categoryVM.category = _unitWork.Categories.GetT(x => x.Id == id);
                if (categoryVM == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(categoryVM);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEditCategory(CategoryVM categoryVM)
        {
            if (ModelState.IsValid)
            {
                if (categoryVM.category.Id == 0)
                {
                    _unitWork.Categories.Add(categoryVM.category);
                    TempData["success"] = "Category Added Successfully !";
                }
                else
                {
                    _unitWork.Categories.Update(categoryVM.category);
                    TempData["success"] = "Category Edited Successfully !";
                }
                _unitWork.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        #endregion

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
