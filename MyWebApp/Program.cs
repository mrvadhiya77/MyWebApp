using Microsoft.EntityFrameworkCore;
using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using MyWebApp.DataAccessLibrary.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyWebApp.CommonHelperRole;
using Stripe;
using MyWebApp.DataAccessLibrary.DBInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//Add Service To Create Login User
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
// Configuration for mysql database
var connectionStrings = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MyWebAppContext>(options =>
{
    options.UseMySql(connectionStrings, ServerVersion.AutoDetect(connectionStrings));
});

// Stripe Configuration
builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection("PaymentSetting"));

//builder.Services.AddDefaultIdentity<IdentityUser>()
//    .AddEntityFrameworkStores<MyWebAppContext>();

// Add Role For User Like Admin Or Customer
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<MyWebAppContext>();

// Add Singleton EmailSender Service For Register Menu
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// For Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
});

// Add Razor Pages Service
builder.Services.AddRazorPages();
// Enable Cache Memory
builder.Services.AddDistributedMemoryCache();
// Add Session
builder.Services.AddSession(options =>
{
    //Session TimeOut
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    //For Session Key Enabled
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//Store Stripe Api SecretKey
StripeConfiguration.ApiKey = builder.Configuration.GetSection("PaymentSetting:SecretKey").Get<string>();
//Call method of this class - Middleware
dataSedding();

app.UseAuthentication();;

app.UseAuthorization();

app.UseSession(); // Use Session
// Razor pages mapping
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void dataSedding()
{
   using(var scope = app.Services.CreateScope())
    {
        var DbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        DbInitializer.Initialize();
    }
}