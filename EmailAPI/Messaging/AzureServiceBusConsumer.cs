using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Shopp.Services.EmailAPI.Message;
using Shopp.Services.EmailAPI.Models.DTO;
using Shopp.Services.EmailAPI.Services;
using Shopp.Services.EmailAPI.Services.IService;
using System.Text;

namespace Shopp.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _emailCartQueue;
        private readonly string _registerUserQueue;
        private readonly string _orderCreated_Topic;
        private readonly string _orderCreated_Email_Subscription;

        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _registerUserProcessor;
        private ServiceBusProcessor _emailOrderPlacedProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;

            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            _emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            _registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");
            _orderCreated_Topic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            _orderCreated_Email_Subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");

            _emailService = emailService;

            var client = new ServiceBusClient(_serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(_emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(_registerUserQueue);
            _emailOrderPlacedProcessor = client.CreateProcessor(_orderCreated_Topic, _orderCreated_Email_Subscription);

        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRecieved;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRecieved;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();

            _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestRecieved;
            _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailOrderPlacedProcessor.StartProcessingAsync();
        }

        private async Task OnOrderPlacedRequestRecieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardMessage reward = JsonConvert.DeserializeObject<RewardMessage>(body);
            try
            {
                await _emailService.LogOrderPlaced(reward);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnUserRegisterRecieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body);
            try
            {
                await _emailService.RegisterUserEmailAndLog(email);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRecieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDTO cart = JsonConvert.DeserializeObject<CartDTO>(body);
            try
            {
                await _emailService.EmailCartAndLog(cart);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();

            await _emailOrderPlacedProcessor.StopProcessingAsync();
            await _emailOrderPlacedProcessor.DisposeAsync();
        }
    }
}
