using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Shopp.Services.CouponAPI.Data;
using Shopp.Services.CouponAPI.Extensions;
using AutoMapper;
using Shopp.Services.CouponAPI.Utils;

namespace Shopp.Services.CouponAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<CouponDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            
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
            Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("PaymentMethod:SecretKey").Get<string>();

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
                    var _bd = scope.ServiceProvider.GetRequiredService<CouponDbContext>();
                    if (_bd.Database.GetPendingMigrations().Count() > 0)
                    {
                        _bd.Database.Migrate();
                    }
                }
            }
        }
    }
}
