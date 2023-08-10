using AutoMapper;
using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using FaterRasanehMarket.Data.Repositories;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public FaterRasanehMarketDBContext _Context { get; }
        private IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContext;
        private ICategoryRepository _categoryRepository;
        private IBrandRepository _brandRepository;
        private IDiscountRepository _discountRepository;
        private ICartAndOrdersRepository _cartAndOrdersRepository;
        private readonly IMapper _mapper;
        public UnitOfWork(IHttpContextAccessor httpContext, IMapper mapper, FaterRasanehMarketDBContext Context)
        {
            _Context = Context;
            _Context.CheckArgumentIsNull(nameof(_Context));
            _httpContext = httpContext;
            _httpContext.CheckArgumentIsNull(nameof(_httpContext));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }
        public IBaseRepository<TEntity> BaseRepository<TEntity>() where TEntity : class
        {
            IBaseRepository<TEntity> repository = new BaseRepository<TEntity, FaterRasanehMarketDBContext>(_Context);
            return repository;
        }
        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                    _productRepository = new ProductRepository(_Context);
                return _productRepository;
            }
        }
        public IBrandRepository BrandRepository
        {
            get
            {
                if (_brandRepository == null)
                    _brandRepository = new BrandRepository(_Context);
                return _brandRepository;
            }
        }
        public IDiscountRepository DiscountRepository
        {
            get
            {
                if (_discountRepository == null)
                    _discountRepository = new DiscountRepository(_Context);
                return _discountRepository;
            }
        }
        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                    _categoryRepository = new CategoryRepository(_Context, _mapper);

                return _categoryRepository;
            }
        }
        public ICartAndOrdersRepository CartAndOrdersRepository
        {
            get
            {
                if (_cartAndOrdersRepository == null)
                    _cartAndOrdersRepository = new CartAndOrdersRepository(_Context);

                return _cartAndOrdersRepository;
            }
        }
        public async Task Commit()
        {
            await _Context.SaveChangesAsync();
        }

    }
}
