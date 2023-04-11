using AutoMapper;
using GeekBurguer.Products.Contract.Dto;
using GeekBurguer.Products.Infra.Repository;
using GeekBurguer.Products.Service.Dto;

namespace GeekBurguer.Products.Service.AutoMapperExtensions.Helper
{
    public class MatchItemsFromRepository :
    IMappingAction<ItemToUpSert, Item>
    {
        private IProductsRepository _productRepository;
        public MatchItemsFromRepository(IProductsRepository
            productRepository)
        {
            _productRepository = productRepository;
        }

        public void Process(ItemToUpSert source, Item destination)
        {
            var fullListOfItems =
                 _productRepository.GetFullListOfItemsAsync().Result;

            var itemFound = fullListOfItems?
                .FirstOrDefault(item => item.Name
                .Equals(source.Name,
                    StringComparison.InvariantCultureIgnoreCase));

            if (itemFound != null)
                destination.ItemId = itemFound.ItemId;
            else
                destination.ItemId = Guid.NewGuid();
        }
    }

}
