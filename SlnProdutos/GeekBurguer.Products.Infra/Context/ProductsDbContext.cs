using GeekBurguer.Products.Contract.Dto;
using Microsoft.EntityFrameworkCore;

namespace GeekBurguer.Products.Infra.Context
{
    public class ProductsDbContext: DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
        : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }

        public DbSet<Item> Items { get; set; }

    }
}
