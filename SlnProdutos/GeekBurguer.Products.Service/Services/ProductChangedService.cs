using AutoMapper;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using GeekBurguer.Products.Contract.Dto;
using GeekBurguer.Products.Infra.Repository;
using GeekBurguer.Products.Service.Configuration;
using GeekBurguer.Products.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;

namespace GeekBurguer.Products.Service.Services
{
    public class ProductChangedService : IProductChangedService
    {
        private const string Topic = "ProductChangedTopic";
        private readonly IConfiguration _configuration;
        private IMapper _mapper;
        private readonly List<ServiceBusMessage> _messages;
        private Task _lastTask;
        //private readonly IServiceBusNamespace _namespace;
        private readonly ILogService _logService;
        private CancellationTokenSource _cancelMessages;
        private IServiceProvider _serviceProvider { get; }

        public ProductChangedService(IMapper mapper,
            IConfiguration configuration, ILogService logService, IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _configuration = configuration;
            _logService = logService;
            _messages = new List<ServiceBusMessage>();

            _cancelMessages = new CancellationTokenSource();
            _serviceProvider = serviceProvider;
            EnsureTopicIsCreated();
        }

        public async Task EnsureTopicIsCreated()
        {
            var config = _configuration.GetSection("serviceBus").Get<ServiceBusConfiguration>();
            var adminClient = new ServiceBusAdministrationClient(config.ConnectionString);
            if (!await adminClient.TopicExistsAsync(Topic))
                await adminClient.CreateTopicAsync(Topic);
        }

        public List<ServiceBusMessage> AddToMessageList(IEnumerable<EntityEntry<Product>> changes)
        {
            var messages = (IEnumerable<ServiceBusMessage>)changes
                .Where(entity => entity.State != EntityState.Detached
                                 && entity.State != EntityState.Unchanged)
                .Select(GetMessage)
                .ToList();

            return messages.ToList();
        }

        private void AddOrUpdateEvent(ProductChangedEvent productChangedEvent)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<IProductChangedEventRepository>();

                    ProductChangedEvent evt;
                    if (productChangedEvent.EventId == Guid.Empty
                        || (evt = scopedProcessingService.Get(productChangedEvent.EventId)) == null)
                        scopedProcessingService.Add(productChangedEvent);
                    else
                    {
                        evt.MessageSent = true;
                        scopedProcessingService.Update(evt);
                    }

                    scopedProcessingService.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public ServiceBusMessage GetMessage(EntityEntry<Product> entity)
        {
            var productChanged = _mapper.Map<ProductChangedMessage>(entity);
            var productChangedSerialized = JsonConvert.SerializeObject(productChanged);
            var productChangedBinaryData = new BinaryData(Encoding.UTF8.GetBytes(productChangedSerialized));

            var productChangedEvent = _mapper.Map<ProductChangedEvent>(entity);
            AddOrUpdateEvent(productChangedEvent);

            return new ServiceBusMessage()
            {
                Body = productChangedBinaryData,
                MessageId = productChangedEvent.EventId.ToString(),
                Subject = productChanged.Product.StoreId.ToString(),

            };
        }

        public async void SendMessagesAsync(List<ServiceBusMessage> messages)
        {
            if (_lastTask != null && !_lastTask.IsCompleted)
                return;

            var config = _configuration.GetSection("serviceBus").Get<ServiceBusConfiguration>();
            var client = new ServiceBusClient(config.ConnectionString);

            _logService.SendMessagesAsync("Product was changed");
            _messages.AddRange(messages);

            var topicSender = client.CreateSender(Topic);
            _lastTask = SendAsync(topicSender, _cancelMessages.Token);

            await _lastTask;

            var closeTask = topicSender.CloseAsync();
            await closeTask;
            HandleException(closeTask);
        }

        public async Task SendAsync(ServiceBusSender topicSender,
            CancellationToken cancellationToken)
        {
            var tries = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_messages.Count <= 0)
                    break;

                ServiceBusMessage message;
                lock (_messages)
                {
                    message = _messages.FirstOrDefault();
                }

                await topicSender.SendMessageAsync(message, cancellationToken);

                //var sendTask = topicSender.SendMessageAsync(message, cancellationToken);
                //await sendTask;
                //var success = HandleException(sendTask);

                //if (!success)
                //{
                //    var cancelled = cancellationToken.WaitHandle.WaitOne(10000 * (tries < 60 ? tries++ : tries));
                //    if (cancelled) break;
                //}
                //else
                //{
                //    if (message == null) continue;
                //    AddOrUpdateEvent(new ProductChangedEvent() { EventId = new Guid(message.MessageId) });
                //    _messages.Remove(message);
                //}
            }
        }

        public bool HandleException(Task task)
        {
            if (task.Exception == null || task.Exception.InnerExceptions.Count == 0) return true;

            task.Exception.InnerExceptions.ToList().ForEach(innerException =>
            {
                Console.WriteLine($"Error in SendAsync task: {innerException.Message}. Details:{innerException.StackTrace} ");

                if (innerException is ServiceBusException)
                    Console.WriteLine("Connection Problem with Host. Internet Connection can be down");
            });

            return false;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            EnsureTopicIsCreated();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancelMessages.Cancel();

            return Task.CompletedTask;
        }
    }
}
