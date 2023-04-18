using Azure.Messaging.ServiceBus;
using GeekBurguer.Products.Contract.Dto;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Hosting;

namespace GeekBurguer.Products.Service.Services.Interfaces
{
    public interface IProductChangedService: IHostedService
    {
        List<ServiceBusMessage> AddToMessageList(IEnumerable<EntityEntry<Product>> changes);
        void SendMessagesAsync(List<ServiceBusMessage> messages);

    }
}
