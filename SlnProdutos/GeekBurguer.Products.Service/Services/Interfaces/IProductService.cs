using GeekBurguer.Products.Service.Dto;

namespace GeekBurguer.Products.Service.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductToGetDto>> GetProductsByStoreNameAsync(string storeName);
        Task<bool> Add(ProductToUpSert productToUpSert);
        Task<ProductToGetDto> GetProductById(Guid id);
    }
}
