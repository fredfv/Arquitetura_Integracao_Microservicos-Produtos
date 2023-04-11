using AutoMapper;
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

        public async Task<List<ProductToGetDto>> GetProductsByStoreNameAsync(string storeName)
        {
            var productsByStore = await _productsRepository.GetProductsByStoreNameAsync(storeName);

            var productsToGet = _mapper.Map<List<ProductToGetDto>>(productsByStore);

            return productsToGet;
        }
    }
}
