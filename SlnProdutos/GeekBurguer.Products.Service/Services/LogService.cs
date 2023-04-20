using AutoMapper;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using GeekBurguer.Products.Service.Configuration;
using GeekBurguer.Products.Service.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace GeekBurguer.Products.Service.Services
{
    public class LogService : ILogService
    {
        private const string MicroService = "Products";
        private const string Topic = "Log";
        private IConfiguration _configuration;
        private IMapper _mapper;
        private List<ServiceBusMessage> _messages;
        private Task _lastTask;
        private ServiceBusConfiguration config;

        public LogService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
            _messages = new List<ServiceBusMessage>();
            config = new ServiceBusConfiguration();
            _configuration.GetSection("serviceBus").Bind(config);
            EnsureTopicIsCreated();
        }

        public async Task EnsureTopicIsCreated()
        {            
            var adminClient = new ServiceBusAdministrationClient(config.ConnectionString);
            if (!await adminClient.TopicExistsAsync(Topic))
                await adminClient.CreateTopicAsync(Topic);
        }


        public ServiceBusMessage GetMessage(string message)
        {
            var productChangedBinaryData = new BinaryData(Encoding.UTF8.GetBytes(message));

            return new ServiceBusMessage
            {
                Body = productChangedBinaryData,
                MessageId = Guid.NewGuid().ToString(),
                Subject = MicroService
            };
        }

        public async void SendMessagesAsync(string message)
        {
            _messages.Add(GetMessage(message));

            if (_lastTask != null && !_lastTask.IsCompleted)
                return;
            
            var client = new ServiceBusClient(config.ConnectionString);

            var topicSender = client.CreateSender(Topic);
            _lastTask = SendAsync(topicSender);

            await _lastTask;

            var closeTask = topicSender.CloseAsync();
            await closeTask;
            HandleException(closeTask);
        }

        public async Task SendAsync(ServiceBusSender topicClient)
        {
            int tries = 0;
            ServiceBusMessage message;
            while (true)
            {
                if (_messages.Count <= 0)
                    break;

                lock (_messages)
                {
                    message = _messages.FirstOrDefault();
                }

                var sendTask = topicClient.SendMessageAsync(message);
                await sendTask;
                var success = HandleException(sendTask);

                if (!success)
                    Thread.Sleep(10000 * (tries < 60 ? tries++ : tries));
                else
                    _messages.Remove(message);
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
    }
}
