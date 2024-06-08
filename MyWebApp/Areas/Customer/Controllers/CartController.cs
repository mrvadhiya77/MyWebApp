using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.CommonHelperRole;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;
using MyWebApp.Models.ViewModels;
using System.Security.Claims;

namespace MyWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartVM vm { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Cart
        public IActionResult Index()
        {
            // Get Logged In User's Identity (Name, ID etc.)
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            // Get NameIdentifier From claimsIdentity
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            vm = new CartVM()
            {
                ListOfCart = _unitOfWork.Carts.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            foreach (var item in vm.ListOfCart)
            {
                // Item wise total
                item.singleItemTotal = (item.Product.Price * item.Count);


                // Cart wise total
                vm.OrderHeader.OrderTotal += item.singleItemTotal;
            }
            return View(vm);
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
            if (cart.Count <= 1)
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
        #endregion

        #region Summary
        public IActionResult Summary()
        {
            // Get Logged In User's Identity (Name, ID etc.)
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            // Get NameIdentifier From claimsIdentity
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            vm = new CartVM()
            {
                ListOfCart = _unitOfWork.Carts.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            // Prepare Order Header
            vm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUsers.GetT(x => x.Id == claim.Value);
            vm.OrderHeader.Name = vm.OrderHeader.ApplicationUser.Name;
            vm.OrderHeader.Phone = vm.OrderHeader.ApplicationUser.PhoneNumber;
            vm.OrderHeader.Address = vm.OrderHeader.ApplicationUser.Address;
            vm.OrderHeader.state = vm.OrderHeader.ApplicationUser.State;
            vm.OrderHeader.Postal = vm.OrderHeader.ApplicationUser.PinCode;

            foreach (var item in vm.ListOfCart)
            {
                // Item wise total
                item.singleItemTotal = (item.Product.Price * item.Count);


                // Cart wise total
                vm.OrderHeader.OrderTotal += item.singleItemTotal;
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult Summary(CartVM vm)
        {
            // Get Logged In User's Identity (Name, ID etc.)
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            // Get NameIdentifier From claimsIdentity
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //Prepare Order Header
            vm.ListOfCart = _unitOfWork.Carts.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product");
            vm.OrderHeader.OrderStatus = OrderStatus.StatusPending;
            vm.OrderHeader.PaymentStatus = PaymentStatus.StatusPending;
            vm.OrderHeader.DateOfOrder = DateTime.Now;
            vm.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var item in vm.ListOfCart)
            {
                // Cart wise total
                vm.OrderHeader.OrderTotal += item.singleItemTotal;
            }

            _unitOfWork.OrderHeaders.Add(vm.OrderHeader);
            _unitOfWork.Save();

            foreach (var item in vm.ListOfCart)
            {
                OrderDetail orderDetails = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = vm.OrderHeader.Id,
                    Count = item.Count,
                    Price = item.Product.Price
                };
                _unitOfWork.OrderDetails.Add(orderDetails);
                _unitOfWork.Save();
            }

            _unitOfWork.Carts.DeleteRange(vm.ListOfCart);
            _unitOfWork.Save();
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
