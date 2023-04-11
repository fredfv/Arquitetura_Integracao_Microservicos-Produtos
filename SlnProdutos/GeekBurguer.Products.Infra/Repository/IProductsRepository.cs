using GeekBurguer.Products.Contract.Dto;

namespace GeekBurguer.Products.Infra.Repository
{
    public interface IProductsRepository: IBaseRepository
    {
        Task<List<Product>> GetProductsByStoreNameAsync(string storeName);
        Task<bool> AddProductAsync(Product product);
        Task<List<Item>> GetFullListOfItemsAsync();
        Task<Product> GetStoreByName(string storeName);
    }

}
