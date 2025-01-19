using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Shopp.Services.RewardAPI.Message;
using Shopp.Services.RewardAPI.Models;
using Shopp.Services.RewardAPI.Services;
using System.Text;

namespace Shopp.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _orderCreatedTopic;
        private readonly string _orderCreatedRewardSubscription;

        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;

        private ServiceBusProcessor _rewardProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;

            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            _orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            _orderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");

            _rewardService = rewardService;

            var client = new ServiceBusClient(_serviceBusConnectionString);
            _rewardProcessor = client.CreateProcessor(_orderCreatedTopic,_orderCreatedRewardSubscription);
            

        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardRequestRecieved;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();
   
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnNewOrderRewardRequestRecieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardMessage rewardMessage = JsonConvert.DeserializeObject<RewardMessage>(body);
            try
            {
                await _rewardService.UpdateRewards(rewardMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();
        }
    }
}
