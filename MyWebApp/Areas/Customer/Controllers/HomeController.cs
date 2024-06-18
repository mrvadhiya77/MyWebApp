using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.DataAccessLibrary.Infrastructure.Repository;
using MyWebApp.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MyWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Products.GetAll(includeProperties: "Category");
            return View(products);
        }

        /// <summary>
        /// Add Details controller and view for product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Details(int? ProductId)
        {
            Cart cart = new Cart()
            {
                Product = _unitOfWork.Products.GetT(p => p.Id == ProductId, includeProperties: "Category"),
                Count = 1,
                ProductId = (int)ProductId
            };
            return View(cart);

        }

        /// <summary>
        /// Post Method For Cart
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Cart cart)
        {
            if (ModelState.IsValid)
            {
                // Get Logged In User's Identity (Name, ID etc.)
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                // Get NameIdentifier From claimsIdentity
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claim.Value;

                // Get Cart Item For Update Case
                var cartItem = _unitOfWork.Carts.GetT(x => x.ProductId == cart.ProductId && x.ApplicationUserId == claim.Value);

                if (cartItem == null)
                {
                    _unitOfWork.Carts.Add(cart);
                    _unitOfWork.Save();
                    //Add Cart item count into session
                    HttpContext.Session.SetInt32("SessionCart", _unitOfWork.Carts.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count);
                }
                else
                {
                    _unitOfWork.Carts.IncrementCartItem(cartItem, cart.Count);
                    _unitOfWork.Save();
                }

                
            }
            return RedirectToAction("Index");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}