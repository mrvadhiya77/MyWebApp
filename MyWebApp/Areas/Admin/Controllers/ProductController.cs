using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models.ViewModels;
namespace MyWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitWork;
        private IWebHostEnvironment _hostingEnvironment;

        public ProductController(IUnitOfWork unitWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitWork = unitWork;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult GetData()
        {
            var products = _unitWork.Products.GetAll(includeProperties:"Category");
            return Json(new { data = products } );
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddEditProduct(int? id)
        {
            ProductVM ProductVM = new ProductVM()
            {
                product = new(),
                CategoryData = _unitWork.Categories.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
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
        public IActionResult AddEditProduct(ProductVM ProductVM, IFormFile? fileUpload)
        {
            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                if (fileUpload != null)
                {
                    string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "Image");
                    fileName = Guid.NewGuid().ToString() + "-" + fileUpload.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        fileUpload.CopyTo(fileStream);
                    }
                    ProductVM.product.ImageUrl = @"\Image\" + fileName;
                }
                if (ProductVM.product.Id == 0)
                {
                    _unitWork.Products.Add(ProductVM.product);
                    TempData["success"] = "Product Added Successfully !";

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
