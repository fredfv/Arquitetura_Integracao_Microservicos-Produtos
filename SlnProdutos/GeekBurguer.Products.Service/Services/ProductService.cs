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
        private IProductChangedService _productChangedService;
        private IMapper _mapper;        

        public ProductService(IProductsRepository productsRepository,
                              IMapper mapper,
                              IProductChangedService productChangedService)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _productChangedService = productChangedService;
        }

        public async Task<bool> Add(ProductToUpSert productToUpSert)
        {
            var product = _mapper.Map<Product>(productToUpSert);
            bool inserted =  await _productsRepository.AddProductAsync(product);
            var messages = _productChangedService.AddToMessageList(_productsRepository.GetContext().ChangeTracker.Entries<Product>());

            await _productsRepository.SaveAsync();
    
            _productChangedService.SendMessagesAsync(messages);

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

        public async Task Update(Guid Id, ProductToUpSert productToUpSert)
        {
            var product = await _productsRepository.GetProductByFilters(f=> f.ProductId == Id);
             product.Store.Name    = productToUpSert.Name;

            var allItemsDB = product.Items.Select(s => s).ToList();

            foreach (var item in allItemsDB)
            {
                product.Items.Remove(item);
                _productsRepository.RemoveItem(item);
            }

            var newItems = _mapper.Map<List<Item>>(productToUpSert.Items);

            foreach (var item in newItems)
            {
                item.Product = product;
                product.Items.Add(item);
                _productsRepository.AddItem(item);
            }

            _productsRepository.UpdateProduct(product);

            var messages = _productChangedService.AddToMessageList(_productsRepository.GetContext().ChangeTracker.Entries<Product>());

            await _productsRepository.SaveAsync();

            _productChangedService.SendMessagesAsync(messages);
        }
    }
}
