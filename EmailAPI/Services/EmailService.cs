using Microsoft.EntityFrameworkCore;
using Shopp.Services.EmailAPI.Data;
using Shopp.Services.EmailAPI.Message;
using Shopp.Services.EmailAPI.Models;
using Shopp.Services.EmailAPI.Models.DTO;
using Shopp.Services.EmailAPI.Services.IService;
using System.Text;

namespace Shopp.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<EmailDbContext> _dbOptions;

        public EmailService(DbContextOptions<EmailDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDTO cart)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/>Cart Email Requested");
            message.AppendLine($"<br/>Total {cart.CartHeader.CartTotal}");
            message.Append("<br/>");
            message.Append("<ul>");
            foreach (var item in cart.CartDetails)
            {
                message.Append("<li>");
                message.Append($"{item.Product.Name} X {item.Count}");
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cart.CartHeader.Email);
        }

        public async Task LogOrderPlaced(RewardMessage reward)
        {
            string message = $"New order placed.<br/> Order ID: {reward.OrderId}";
            await LogAndEmail(message, "maximgromov@gmail.com");
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = $"User registration successful! <br/> Email: {email}";
            await LogAndEmail(message, "maximgromov@gmail.com");
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger logger = new EmailLogger()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };

                await using var db = new EmailDbContext(_dbOptions);
                await db.Loggers.AddAsync(logger);
                await db.SaveChangesAsync();

                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }
    }
}
