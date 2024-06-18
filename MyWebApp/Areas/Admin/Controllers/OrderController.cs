using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.CommonHelperRole;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.DataAccessLibrary.Infrastructure.Repository;
using MyWebApp.Models;
using MyWebApp.Models.ViewModels;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace MyWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get Order Data
        /// </summary>
        /// <returns></returns>
        public IActionResult GetData(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                // All Data For Employee and Admin
                orderHeaders = _unitOfWork.OrderHeaders.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                // Get Logged In User's Identity (Name, ID etc.)
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                // Get NameIdentifier From claimsIdentity
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                // To GetData For Perticular login user data
                orderHeaders = _unitOfWork.OrderHeaders.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == PaymentStatus.StatusPending);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == PaymentStatus.StatusApproved);
                    break;
                case "underprocess":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == OrderStatus.StatusInProgres);
                    break;
                case "shipped":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == OrderStatus.StatusShipped);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }

        /// <summary>
        /// Order's Items Details
        /// </summary>
        /// <returns></returns>
        public IActionResult OrderDetail(int id)
        {
            OrderVM orderVM = new OrderVM()
            {

                OrderHeader = _unitOfWork.OrderHeaders.GetT(x => x.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(x => x.OrderHeader.Id == id, includeProperties: "Product")
            };
            return View(orderVM);

        }

        [HttpPost]
        [Authorize(Roles = WebsiteRole.Role_Admin + ", " + WebsiteRole.Role_Employee)]
        public IActionResult OrderDetail(OrderVM orderVM)
        {
            var orderHeader = _unitOfWork.OrderHeaders.GetT(x => x.Id == orderVM.OrderHeader.Id);
            orderHeader.Name = orderVM.OrderHeader.Name;
            orderHeader.Phone = orderVM.OrderHeader.Phone;
            orderHeader.Address = orderVM.OrderHeader.Address;
            orderHeader.City = orderVM.OrderHeader.City;
            orderHeader.state = orderVM.OrderHeader.state;
            orderHeader.Postal = orderVM.OrderHeader.Postal;

            if (orderVM.OrderHeader.Carrier != null)
            {
                orderHeader.Carrier = orderVM.OrderHeader.Carrier;
            }

            if (orderVM.OrderHeader.TrackingNumber != null)
            {
                orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeaders.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Info Updated Successfully";

            return RedirectToAction("OrderDetail", "Order", new { id = orderVM.OrderHeader.Id });

        }

        [Authorize(Roles = WebsiteRole.Role_Admin + ", " + WebsiteRole.Role_Employee)]
        public IActionResult InProcess(OrderVM orderVM)
        {
            _unitOfWork.OrderHeaders.UpdateStatus(orderVM.OrderHeader.Id, OrderStatus.StatusInProgres);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated-InProcess";

            return RedirectToAction("OrderDetail", "Order", new { id = orderVM.OrderHeader.Id });

        }

        [Authorize(Roles = WebsiteRole.Role_Admin + ", " + WebsiteRole.Role_Employee)]
        public IActionResult Shipped(OrderVM orderVM)
        {
            var orderHeader = _unitOfWork.OrderHeaders.GetT(x => x.Id == orderVM.OrderHeader.Id);
            orderHeader.Carrier = orderVM.OrderHeader.Carrier;
            orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = OrderStatus.StatusShipped;
            orderHeader.DateOfShipping = DateTime.Now;
            _unitOfWork.OrderHeaders.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated-Shipped";

            return RedirectToAction("OrderDetail", "Order", new { id = orderVM.OrderHeader.Id });

        }

        [Authorize(Roles = WebsiteRole.Role_Admin + ", " + WebsiteRole.Role_Employee)]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            var orderHeader = _unitOfWork.OrderHeaders.GetT(x => x.Id == orderVM.OrderHeader.Id);

            // Check payment status approved for cancel order for refund creation
            if (orderHeader.PaymentStatus == PaymentStatus.StatusApproved)
            {

                //var refundOptions = new RefundCreateOptions
                //{
                //    Reason = RefundReasons.RequestedByCustomer,
                //    PaymentIntent = orderHeader.PaymentIntentId
                //};
                //var service = new RefundService();
                //Refund refund = service.Create(refundOptions);
                _unitOfWork.OrderHeaders.UpdateStatus(orderVM.OrderHeader.Id, OrderStatus.StatusCalcelled, OrderStatus.StatusRefund);
            }
            else
            {
                _unitOfWork.OrderHeaders.UpdateStatus(orderVM.OrderHeader.Id, OrderStatus.StatusCalcelled, OrderStatus.StatusCalcelled);
            }
            _unitOfWork.OrderHeaders.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled";

            return RedirectToAction("OrderDetail", "Order", new { id = orderVM.OrderHeader.Id });

        }

        public IActionResult PayNow(OrderVM vm)
        {

            var OrderHeader = _unitOfWork.OrderHeaders.GetT(x => x.Id == vm.OrderHeader.Id, includeProperties: "ApplicationUser");
            var OrderDetails = _unitOfWork.OrderDetails.GetAll(x => x.OrderHeader.Id == vm.OrderHeader.Id, includeProperties: "Product");

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
            foreach (var item in OrderDetails)
            {
                var lineItemsOptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
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
            return RedirectToAction("Index", "Home");
        }
    }
}
