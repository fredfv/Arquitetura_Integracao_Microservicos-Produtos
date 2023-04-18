using GeekBurguer.Products.Contract.Dto;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeekBurguer.Products.Service.Services.Interfaces
{
    public interface IProductChangedService
    {
        void AddToMessageList(IEnumerable<EntityEntry<Product>> changes);
        Task SendMessageAsync();

    }
}
