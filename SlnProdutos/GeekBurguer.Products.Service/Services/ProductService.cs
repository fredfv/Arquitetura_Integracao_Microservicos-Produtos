using AutoMapper;
using GeekBurguer.Products.Contract.Dto;
using GeekBurguer.Products.Infra.Repository;
using GeekBurguer.Products.Service.Dto;
using GeekBurguer.Products.Service.Services.Interfaces;

namespace GeekBurguer.Products.Service.Services
{
    public class ProductService: IProductService
    {
        private readonly IProductsRepository _productsRepository;
        private IMapper _mapper;        

        public ProductService(IProductsRepository productsRepository, IMapper mapper)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
        }

        public async Task<bool> Add(ProductToUpSert productToUpSert)
        {
            var product = _mapper.Map<Product>(productToUpSert);
            bool inserted =  await _productsRepository.AddProductAsync(product);
            await _productsRepository.SaveAsync();

            return inserted;
        }

        public async Task<ProductToGetDto> GetProductById(Guid id)
        {
            var productDb = await _productsRepository.GetProductByFilters(f => f.ProductId == id);
            var productMap = _mapper.Map<ProductToGetDto>(productDb);

            return productMap;
        }

        public async Task<List<ProductToGetDto>> GetProductsByStoreNameAsync(string storeName)
        {
            var productsByStore = await _productsRepository.GetProductsByStoreNameAsync(storeName);

            var productsToGet = _mapper.Map<List<ProductToGetDto>>(productsByStore);

            return productsToGet;
        }
    }
}
