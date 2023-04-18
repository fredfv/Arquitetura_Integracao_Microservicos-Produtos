using AutoMapper;
using GeekBurguer.Products.Contract.Dto;
using GeekBurguer.Products.Service.Configuration;
using GeekBurguer.Products.Service.Dto;
using GeekBurguer.Products.Service.Services.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace GeekBurguer.Products.Service.Services
{
    public class ProductChangedService : IProductChangedService
    {
        private List<Message> _messages;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusConfiguration _serviceBusConfiguration;

        public ProductChangedService(IConfiguration configuration, IOptions<ServiceBusConfiguration> serviceBusConfiguration)
        {
            _messages = new List<Message>();
            _configuration = configuration;
            _serviceBusConfiguration = serviceBusConfiguration.Value;
        }

        public void AddToMessageList(IEnumerable<EntityEntry<Product>> changes)
        {
            _messages.Clear();

            _messages.AddRange(changes
                     .Where(entity =>
                       entity.State != EntityState.Detached
                       && entity.State != EntityState.Unchanged)
                     .Select(entity => GetMessage(entity)));

        }

        private Message GetMessage(EntityEntry<Product> entity)
        {
            var productChanged =
                Mapper.Map<ProductToGetDto>(entity);

            var productChangedSerialized =
                JsonConvert.SerializeObject(productChanged);

            var productChangedByteArray =
               Encoding.UTF8.GetBytes(productChangedSerialized);

            return new Message
            {
                Body = productChangedByteArray,
                MessageId = Guid.NewGuid().ToString(),
                Label = productChanged.StoreId.ToString()
            };
        }

        public async Task SendMessageAsync()
        {
            var connectionString = _configuration
             ["serviceBus:ConnectionString"];
            var queueClient = new QueueClient
                (connectionString, "ProductChanged");

            int tries = 0;
            Message message;
            while (true)
            {
                if ((_messages.Count <= 0) || (tries > 10))
                    break;

                lock (_messages)
                {
                    message = _messages.FirstOrDefault();
                }

                await queueClient.SendAsync(message);
                _messages.Remove(message);
            }

            await queueClient.CloseAsync();


        }

        private async void ReceiveMessages()
        {
            if (_serviceBusConfiguration != null)
            {
                string TopicName = "";
                string SubscriptionName = "";

                var subscriptionClient = new SubscriptionClient(_serviceBusConfiguration.ConnectionString,
                                                                TopicName,
                                                                SubscriptionName);

                //by default a 1=1 rule is added when subscription is created, so we need to remove it
                await subscriptionClient.RemoveRuleAsync("$Default");

                await subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = "filter-store" },
                    Name = "filter-store"
                });

                var mo = new MessageHandlerOptions(ExceptionHandler) { AutoComplete = true };

                subscriptionClient.RegisterMessageHandler(MessageHandler, mo);

                Console.ReadLine();
            }

        }

        private static Task MessageHandler(Message message, CancellationToken arg2)
        {
            Console.WriteLine($"message Label: {message.Label}");
            Console.WriteLine($"CorrelationId: {message.CorrelationId}");
            var prodChangesString = Encoding.UTF8.GetString(message.Body);

            Console.WriteLine("Message Received");
            Console.WriteLine(prodChangesString);

            //Thread.Sleep(40000);

            return Task.CompletedTask;
        }

        private static Task ExceptionHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Handler exception {arg.Exception}.");
            var context = arg.ExceptionReceivedContext;
            Console.WriteLine($"Endpoint: {context.Endpoint},Path: {context.EntityPath}, Action: {context.Action} ");
            return Task.CompletedTask;
        }
    }
}
