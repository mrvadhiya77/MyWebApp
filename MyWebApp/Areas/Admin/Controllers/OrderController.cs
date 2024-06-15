﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.CommonHelperRole;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;
using MyWebApp.Models.ViewModels;
using System.Security.Claims;

namespace MyWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitWork;

        public OrderController(IUnitOfWork unitWork)
        {
            _unitWork = unitWork;
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
                orderHeaders = _unitWork.OrderHeaders.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                // Get Logged In User's Identity (Name, ID etc.)
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                // Get NameIdentifier From claimsIdentity
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                // To GetData For Perticular login user data
                orderHeaders = _unitWork.OrderHeaders.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == PaymentStatus.StatusPending);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == PaymentStatus.StatusApproved);
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

                OrderHeader = _unitWork.OrderHeaders.GetT(x => x.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitWork.OrderDetails.GetAll(x=> x.Id ==id,includeProperties:"Product")
            };
            return View(orderVM);

        }
    }
}
