using Microsoft.AspNetCore.Mvc;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.Models;
using System.Security.Claims;

namespace MyWebApp.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public readonly IUnitOfWork _unitOfWork;

        public CartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get Logged In User's Identity (Name, ID etc.)
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            // Get NameIdentifier From claimsIdentity
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //if Claim is not null then get session value for cart otherwise clear it
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32("SessionCart") != null)
                {
                    return View(HttpContext.Session.GetInt32("SessionCart"));
                }
                else
                {
                    HttpContext.Session.SetInt32("SessionCart", _unitOfWork.Carts.GetAll(a => a.ApplicationUserId == claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32("SessionCart"));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
