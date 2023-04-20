using AutoMapper;
using GeekBurguer.Products.Infra.Context;
using GeekBurguer.Products.Infra.Extension;
using GeekBurguer.Products.Infra.Repository;
using GeekBurguer.Products.Service.AutoMapperExtensions;
using GeekBurguer.Products.Service.Configuration;
using GeekBurguer.Products.Service.Services;
using GeekBurguer.Products.Service.Services.Interfaces;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeekBurguer.Products.IoC
{
    public static class Bootstrap
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddDbContext<ProductsDbContext>(o => o.UseInMemoryDatabase("geekburger-products"));

            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductChangedService, ProductChangedService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IProductChangedEventRepository, ProductChangedEventRepository>();

            var serviceProvider = services.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();

            services.AddAutoMapper( ac => {
                ac.AddProfile<AutoMapperProfile>();
            });

            var productContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
            productContext.Seed();

            //services.AddSingleton<IServiceBusNamespace>(provider =>
            //{
            //    var configuration = provider.GetService<IConfiguration>();
            //    var serviceBusNamespace = configuration.GetServiceBusNamespace();
            //    serviceBusNamespace.SetupTopic(configuration);

            //    return serviceBusNamespace;
            //});
         
            return services;
        }

        private static IServiceBusNamespace GetServiceBusNamespace(this IConfiguration configuration)
        {
            var config = new ServiceBusConfiguration();

            configuration.GetSection("serviceBus").Bind(config);

            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(config.ClientId,
                               config.ClientSecret,
                               config.TenantId,
                               AzureEnvironment.AzureGlobalCloud);

            var serviceBusManager = ServiceBusManager
                .Authenticate(credentials, config.SubscriptionId);
            return serviceBusManager.Namespaces
                   .GetByResourceGroup(config.ResourceGroup,
                   config.NamespaceName);
        }

        private static void SetupTopic(this IServiceBusNamespace serviceBusNamespace, IConfiguration _configuration)
        {
            const string TopicName = "ProductChangedTopic";
            const string SubscriptionName = "paulista_store";

            if (!serviceBusNamespace.Topics.List()
                  .Any(t => t.Name
                  .Equals(TopicName, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                serviceBusNamespace.Topics
                                    .Define(TopicName)
                                    .WithSizeInMB(1024)
                                    .Create();
                            }

            var topic = serviceBusNamespace.Topics.GetByName(TopicName);

            if (topic.Subscriptions.List()
              .Any(subscription => subscription.Name
              .Equals(SubscriptionName,
                     StringComparison.InvariantCultureIgnoreCase)))
            {
                topic.Subscriptions.DeleteByName(SubscriptionName);
            }

            topic.Subscriptions
                .Define(SubscriptionName)
                .Create();
        }

    }
}