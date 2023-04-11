using AutoMapper;
using GeekBurguer.Products.Contract.Dto;
using GeekBurguer.Products.Infra.Repository;
using GeekBurguer.Products.Service.Dto;

namespace GeekBurguer.Products.Service.AutoMapperExtensions.Helper
{
    public class MatchStoreFromRepository :
    IMappingAction<ProductToUpSert, Product>
    {
        private IProductsRepository _productsRepository;
        public MatchStoreFromRepository(IProductsRepository
            productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public void Process(ProductToUpSert source,
            Product destination)
        {
            var store =
                 _productsRepository.GetStoreByName(source.StoreName).Result;

            if (store != null)
                destination.StoreId = store.StoreId;
        }
    }

}
