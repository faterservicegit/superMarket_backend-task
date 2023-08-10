using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Contracts
{
    public interface IUnitOfWork
    {
        IBaseRepository<TEntity> BaseRepository<TEntity>() where TEntity : class;
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IDiscountRepository DiscountRepository { get; }
        IBrandRepository BrandRepository { get; }
        ICartAndOrdersRepository CartAndOrdersRepository { get; }
        FaterRasanehMarketDBContext _Context { get; }
        Task Commit();
    }
}
