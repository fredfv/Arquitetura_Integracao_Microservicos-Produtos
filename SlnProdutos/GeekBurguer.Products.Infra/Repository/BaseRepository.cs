using GeekBurguer.Products.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GeekBurguer.Products.Infra.Repository
{
    public class BaseRepository<T>: IBaseRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        public BaseRepository(DbContext context)
        {
            _dbContext = context;
        }

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public IQueryable<T> GetByFilters(bool tracking = false, Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IQueryable<T>> func = null)
        {
            IQueryable<T> result = _dbContext.Set<T>();

            if (!tracking)
                result = result.AsNoTracking();

            if (func != null)
                result = func(result);

            if (predicate == null)
                return result;
            else
            {
                result = result.Where(predicate);
                return result;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(_dbContext);
        }
    }
}
