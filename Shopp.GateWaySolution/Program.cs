using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;
using Shopp.GateWaySolution.Extensions;

namespace Shopp.GateWaySolution
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddAppAuthentication();
            builder.Services.AddOcelot();


            var app = builder.Build();

            

            app.MapGet("/", () => "Hello World!");

            app.UseOcelot();
            app.Run();
        }
    }
}
