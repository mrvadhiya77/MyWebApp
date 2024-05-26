using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models.ViewModels;
using System.Security.Claims;

namespace MyWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartVM itemList { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // Get Logged In User's Identity (Name, ID etc.)
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            // Get NameIdentifier From claimsIdentity
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            itemList = new CartVM()
            {
                ListOfCart = _unitOfWork.Carts.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            foreach (var item in itemList.ListOfCart)
            {
                // Item wise total
                item.singleItemTotal = (item.Product.Price * item.Count);

                // Cart wise total
                itemList.Total += item.singleItemTotal;
            }
            return View(itemList);
        }

        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.Carts.GetT(x => x.Id == id);
            _unitOfWork.Carts.IncrementCartItem(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.Carts.GetT(x => x.Id == id);
            if (cart.Count <= 1 )
            {
                _unitOfWork.Carts.Delete(cart);
            }
            else
            {
                _unitOfWork.Carts.DecrementCartItem(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.Carts.GetT(x => x.Id == id);
            _unitOfWork.Carts.Delete(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
