using System.Linq.Expressions;

namespace GeekBurguer.Products.Infra.Repository
{
    public interface IBaseRepository<T>: IDisposable
    {
        Task<int> SaveAsync();
        IQueryable<T> GetByFilters(bool tracking = false,
                                    Expression<Func<T, bool>> predicate = null,
                                    Func<IQueryable<T>, 
                                    IQueryable<T>> func = null);
        
    }
}
