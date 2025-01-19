using Shopp.Services.EmailAPI.Messaging;
using System.Reflection.Metadata;

namespace Shopp.Services.EmailAPI.Extensions
{
    public static class ApplicationBuilderExtension
    {
        private static IAzureServiceBusConsumer _serviceBusConsumer {  get; set; }  
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app) 
        {
            _serviceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostAppLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostAppLife.ApplicationStarted.Register(OnStart);
            hostAppLife.ApplicationStopping.Register(OnStop);

            return app;
        }

        private static void OnStop()
        {
            _serviceBusConsumer.Stop();
        }

        private static void OnStart()
        {
            _serviceBusConsumer.Start();
        }
    }
}
