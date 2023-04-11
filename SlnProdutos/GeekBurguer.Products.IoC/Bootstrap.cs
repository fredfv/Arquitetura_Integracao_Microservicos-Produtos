using AutoMapper;
using GeekBurguer.Products.Infra.Context;
using GeekBurguer.Products.Infra.Extension;
using GeekBurguer.Products.Infra.Repository;
using GeekBurguer.Products.Service.AutoMapperExtensions;
using GeekBurguer.Products.Service.Services;
using GeekBurguer.Products.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeekBurguer.Products.IoC
{
    public static class Bootstrap
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddDbContext<ProductsDbContext>(o => o.UseInMemoryDatabase("geekburger-products"));

            services
              .AddScoped<IProductsRepository, ProductsRepository>();

            services.AddScoped<IProductService, ProductService>();

            var serviceProvider = services.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();

            services.AddAutoMapper( ac => {
                ac.AddProfile<AutoMapperProfile>();
            });

            var productContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

            productContext.Seed();

            return services;
        }
    }
}