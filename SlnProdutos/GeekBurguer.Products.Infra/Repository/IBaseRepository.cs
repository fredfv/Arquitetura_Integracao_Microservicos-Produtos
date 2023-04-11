namespace GeekBurguer.Products.Infra.Repository
{
    public interface IBaseRepository: IDisposable
    {
        Task SaveAsync();
    }
}
