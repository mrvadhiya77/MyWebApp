using Microsoft.AspNetCore.Mvc;
using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;
using MyWebApp.Models.ViewModels;
namespace MyWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitWork;

        public ProductController(IUnitOfWork unitWork)
        {
            _unitWork = unitWork;
        }

        public IActionResult Index()
        {
            ProductVM productVM = new ProductVM();
            productVM.products = _unitWork.Products.GetAll();
            return View(productVM);
        }

        [HttpGet]
        public IActionResult AddEditProduct(int? id)
        {
            ProductVM ProductVM = new ProductVM();
            if (id == null || id == 0)
            {
                return View(ProductVM);
            }
            else
            {
                ProductVM.product = _unitWork.Products.GetT(x => x.Id == id);
                if (ProductVM == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(ProductVM);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEditProduct(ProductVM ProductVM)
        {
            if (ModelState.IsValid)
            {
                if (ProductVM.product.Id == 0)
                {
                    _unitWork.Products.Add(ProductVM.product);
                    TempData["success"] = "Product Added Successfully !";
                }
                else
                {
                    _unitWork.Products.Update(ProductVM.product);
                    TempData["success"] = "Product Edited Successfully !";
                }
                _unitWork.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
     

        [HttpGet]
        public IActionResult DeleteProduct(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var product = _unitWork.Products.GetT(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteData(int? id)
        {
            var product = _unitWork.Products.GetT(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _unitWork.Products.Delete(product);
            _unitWork.Save();
            TempData["success"] = "Product Deleted Successfully !";
            return RedirectToAction("Index");
        }
    }
}
