using Microsoft.EntityFrameworkCore;

namespace GeekBurguer.Products.Infra.Repository
{
    public class BaseRepository: IBaseRepository
    {
        private readonly DbContext _dbContext;
        public BaseRepository(DbContext context)
        {
            _dbContext = context;
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(_dbContext);
        }
    }
}
