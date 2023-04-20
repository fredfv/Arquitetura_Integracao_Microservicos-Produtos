using AutoMapper;
using GeekBurguer.Products.Contract.Dto;
using GeekBurguer.Products.Service.AutoMapperExtensions.Helper;
using GeekBurguer.Products.Service.Dto;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeekBurguer.Products.Service.AutoMapperExtensions
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductToGetDto>();
            CreateMap<Item, ItemToGetDto>();

            CreateMap<ProductToUpSert, Product>()
             .AfterMap<MatchStoreFromRepository>();
            CreateMap<ItemToUpSert, Item>()
                .AfterMap<MatchItemsFromRepository>();

            CreateMap<EntityEntry<Product>, ProductChangedMessage>()
                    .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Entity));
            CreateMap<EntityEntry<Product>, ProductChangedEvent>()
                    .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Entity));

        }
    }
}
