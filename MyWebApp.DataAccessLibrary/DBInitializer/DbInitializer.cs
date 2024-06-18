using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWebApp.CommonHelperRole;
using MyWebApp.DataAccesLayer.Data;
using MyWebApp.Models;

namespace MyWebApp.DataAccessLibrary.DBInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyWebAppContext _context;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, MyWebAppContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {
                throw;
            }
            //Check User Role Of Admin
            if (!_roleManager.RoleExistsAsync(WebsiteRole.Role_Admin).GetAwaiter().GetResult())
            {
                // Create Admin Role
                _roleManager.CreateAsync(new IdentityRole(WebsiteRole.Role_Admin)).GetAwaiter().GetResult();
                // Create User Role
                _roleManager.CreateAsync(new IdentityRole(WebsiteRole.Role_User)).GetAwaiter().GetResult();
                // Create Employee Role
                _roleManager.CreateAsync(new IdentityRole(WebsiteRole.Role_Employee)).GetAwaiter().GetResult();

                //Create User
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Admin",
                    PhoneNumber = "5864123695",
                    Address = "abc",
                    City = "Newyork",
                    State = "Florida",
                    PinCode = "541235"
                }, "Admin@123").GetAwaiter().GetResult();

                // Get Created User
                ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");

                //assign User Role
                _userManager.AddToRoleAsync(user, WebsiteRole.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
