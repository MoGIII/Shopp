using Microsoft.AspNetCore.Authentication.Cookies;
using Shopp.Web.Service;
using Shopp.Web.Service.IService;
using Shopp.Web.Utility;

namespace Shopp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<ICouponService,CouponService>();
            builder.Services.AddHttpClient<IAuthService, AuthService>();
            builder.Services.AddHttpClient<IProductService, ProductService>();
            builder.Services.AddHttpClient<ICartService, CartService>();
            builder.Services.AddHttpClient<IOrderService, OrderService>();

            StaticData.CouponAPIBase = builder.Configuration["ServicesUrls:CouponAPI"];
            StaticData.AuthAPIBase = builder.Configuration["ServicesUrls:AuthAPI"];
            StaticData.ProductAPIBase = builder.Configuration["ServicesUrls:ProductAPI"];
            StaticData.CartAPIBase = builder.Configuration["ServicesUrls:ShoppingCartAPI"];
            StaticData.OrderAPIBase = builder.Configuration["ServicesUrls:OrderAPI"];

            builder.Services.AddScoped<IBaseService, BaseService>();
            builder.Services.AddScoped<ICouponService, CouponService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(10);
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
