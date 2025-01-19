using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shopp.Services.ShoppingCartAPI.Data;
using System.Text;
using Shopp.Services.ShoppingCartAPI.Extensions;
using Shopp.Services.ShoppingCartAPI.Utils;
using Shopp.Services.ShoppingCartAPI.Services.IService;
using Shopp.Services.ShoppingCartAPI.Services;
using Shopp.MessageBus;

namespace Shopp.Services.CouponAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ShoppingCartDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
            builder.Services.AddSingleton(mapper);
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICouponService, CouponService>();
            builder.Services.AddScoped<IMessageBus,MessageBus.MessageBus>();

            builder.Services.AddHttpClient("Product", u => u.BaseAddress = new Uri(builder.Configuration["ServicesUrls:ProductAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();
            builder.Services.AddHttpClient("Coupon", u => u.BaseAddress = new Uri(builder.Configuration["ServicesUrls:CouponAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string as following: `Bearer Generated_JWT_Token`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },new string[] {}
                    }
                });
            });

            builder.AddAppAuthentication();

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            ApplyMigration();

            app.Run();


            void ApplyMigration()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var _bd = scope.ServiceProvider.GetRequiredService<ShoppingCartDbContext>();
                    if (_bd.Database.GetPendingMigrations().Count() > 0)
                    {
                        _bd.Database.Migrate();
                    }
                }
            }
        }
    }
}
