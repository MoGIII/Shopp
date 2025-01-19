using Microsoft.EntityFrameworkCore;
using Shopp.Services.EmailAPI.Models;

namespace Shopp.Services.EmailAPI.Data
{
    public class EmailDbContext: DbContext
    {
        public DbSet<EmailLogger> Loggers { get; set; }
        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options)
        {

        }
    }

}
