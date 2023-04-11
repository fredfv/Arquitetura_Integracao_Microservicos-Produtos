using GeekBurguer.Products.Contract.Dto;
using GeekBurguer.Products.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekBurguer.Products.Infra.Repository
{
    public class ProductsRepository : BaseRepository, IProductsRepository
    {
        private ProductsDbContext _context;

        public ProductsRepository(ProductsDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            product.ProductId = Guid.NewGuid();
            await _context.Products.AddAsync(product);
            return true;
        }

        public async Task<List<Item>> GetFullListOfItemsAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<List<Product>>
            GetProductsByStoreNameAsync(string storeName)
        {
            var products = await _context.Products?
            .Where(product =>
                product.Store.Name.Equals(storeName,
                StringComparison.InvariantCultureIgnoreCase))
            .Include(product => product.Items)
            .ToListAsync();

            return products;
        }

        public async Task<Product> GetStoreByName(string storeName)
        {
            return await _context.Products?.FirstOrDefaultAsync(f => f.Store.Name == storeName);
        }
    }

}
