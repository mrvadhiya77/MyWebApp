 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.CommonHelperRole;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;
using MyWebApp.Models.ViewModels;
using Stripe.Checkout;
using Stripe;
using System.Security.Claims;
using System.Diagnostics;

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
                var count = _unitOfWork.Carts.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32("SessionCart", count);
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

            var count = _unitOfWork.Carts.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32("SessionCart", count);
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

            #region Stripe Payment Checkout
            //Web Url
            var domain = "https://localhost:7161/";

            // Add Customer Details FOr International transaction
            //var custOptions = new CustomerCreateOptions
            //{
            //    Name = "John Doe",
            //    Address = new AddressOptions
            //    {
            //        Line1 = "510 Townsend St",
            //        PostalCode = "98140",
            //        City = "San Francisco",
            //        State = "CA",
            //        Country = "US",
            //    },
            //};
            //var custommerService = new CustomerService();
            //custommerService.Create(custOptions);

            // Session Create For Cart Item
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Customer/Cart/OrderSuccess?id={vm.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/Index",
            };

            // Store Purchase Product Details To Stripe Session
            foreach (var item in vm.ListOfCart)
            {
                var lineItemsOptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price*100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = item.Count
                };

                // Store Cart Product Data To SessionCreateOptions
                options.LineItems.Add(lineItemsOptions);
            }


            var service = new SessionService();
            Session session = service.Create(options);

            // Call PaymentStaus Method Of OrderHeader
            _unitOfWork.OrderHeaders.PaymentStatus(vm.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            #endregion

            //_unitOfWork.Carts.DeleteRange(vm.ListOfCart);
            //_unitOfWork.Save();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult OrderSuccess(int id)
        {
            //"System.InvalidCastException: 'Unable to cast object of type 'System.DBNull' to type 'System.String'.'"
            //Error Occur If Database Store Null Value And class Property does not set Nullable
            var orderHeader = _unitOfWork.OrderHeaders.GetT(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeaders.UpdateStatus(id, OrderStatus.StatusApproved, PaymentStatus.StatusApproved);
            }

            List<Cart> carts = _unitOfWork.Carts.GetAll(x => x.Id == id).ToList();
            _unitOfWork.Carts.DeleteRange(carts);
            _unitOfWork.Save();
            return View(id);
        }
        #endregion
    }
}
