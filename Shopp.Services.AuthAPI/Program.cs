using Shopp.Services.AuthAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Shopp.Services.AuthAPI.Models;
using Shopp.Services.AuthAPI.Service.IService;
using Shopp.Services.AuthAPI.Service;
using Shopp.MessageBus;

namespace Shopp.Services.AuthAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();

            builder.Services.AddControllers();

            builder.Services.AddScoped<IAuthService,AuthService>();
            builder.Services.AddScoped<IJWTTokenGenerator,JWTTokenGenerator>();
            builder.Services.AddScoped<IMessageBus, MessageBus.MessageBus>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
                    var _bd = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                    if (_bd.Database.GetPendingMigrations().Count() > 0)
                    {
                        _bd.Database.Migrate();
                    }
                }
            }
        }

    }
}
