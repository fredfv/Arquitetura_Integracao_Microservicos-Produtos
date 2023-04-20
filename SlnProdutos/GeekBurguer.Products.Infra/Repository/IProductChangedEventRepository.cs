using GeekBurguer.Products.Contract.Dto;

namespace GeekBurguer.Products.Infra.Repository
{
    public interface IProductChangedEventRepository
    {
        ProductChangedEvent Get(Guid eventId);
        bool Add(ProductChangedEvent productChangedEvent);
        bool Update(ProductChangedEvent productChangedEvent);
        void Save();
    }
}
