using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.CommonHelperRole;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;
using MyWebApp.Models.ViewModels;
using System.Timers;

namespace MyWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebsiteRole.Role_Admin)]
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
            var products = _unitWork.Products.GetAll(includeProperties: "Category");
            return Json(new { data = products });
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
        public IActionResult AddEditProduct(ProductVM productVM, IFormFile? fileUpload)
        {
            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                if (fileUpload != null)
                {
                    // Create Path for new product image store
                    string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "Image");
                    fileName = Guid.NewGuid().ToString() + "-" + fileUpload.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);

                    if (productVM.product.ImageUrl != null)
                    {
                        // Delete Image Physically When Product was edited
                        var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, productVM.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    //Store New Image To Created Path For Add and Edit 
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        fileUpload.CopyTo(fileStream);
                    }
                    productVM.product.ImageUrl = @"\Image\" + fileName;
                }
                if (productVM.product.Id == 0)
                {
                    _unitWork.Products.Add(productVM.product);
                    TempData["success"] = "Product Added Successfully !";
                }
                else
                {
                    _unitWork.Products.Update(productVM.product);
                    TempData["success"] = "Product Updated Successfully !";
                }
                _unitWork.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


        //[HttpGet]
        //public IActionResult DeleteProduct(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var product = _unitWork.Products.GetT(x => x.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitWork.Products.GetT(x => x.Id == id);
            if (product == null)
            {
                return Json(new { success = false, messege = "Error in Fetching Data" });
            }
            else
            {
                // Delete Image Physically From wwwroot
                var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                //Delete Record From DataBase
                _unitWork.Products.Delete(product);
                _unitWork.Save();
                return Json(new { success = true, messege = "Product Deleted Successfully" });
            }
            //TempData["success"] = "Product Deleted Successfully !";
            //return RedirectToAction("Index");
        }
    }
}
