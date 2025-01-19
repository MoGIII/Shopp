
using Microsoft.EntityFrameworkCore;
using Shopp.Services.EmailAPI.Data;
using Shopp.Services.EmailAPI.Extensions;
using Shopp.Services.EmailAPI.Messaging;
using Shopp.Services.EmailAPI.Services;
using Shopp.Services.EmailAPI.Services.IService;

namespace EmailAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<EmailDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var optionsBuilder= new DbContextOptionsBuilder<EmailDbContext>();
            optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

            builder.Services.AddSingleton(new EmailService(optionsBuilder.Options));
            builder.Services.AddSingleton<IAzureServiceBusConsumer,  AzureServiceBusConsumer>();

            builder.Services.AddControllers();
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

            app.UseAuthorization();


            app.MapControllers();

            ApplyMigration();

            app.UseAzureServiceBusConsumer();

            app.Run();

            void ApplyMigration()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var _bd = scope.ServiceProvider.GetRequiredService<EmailDbContext>();
                    if (_bd.Database.GetPendingMigrations().Count() > 0)
                    {
                        _bd.Database.Migrate();
                    }
                }
            }
        }
    }
}
