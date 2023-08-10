using FaterRasanehMarket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Contracts
{
    public interface IBaseRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> FindAllAsync();
        IEnumerable<TEntity> FindAll();
        Task<TEntity> FindByIdAsync(object id);
        Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderdBy = null, params Expression<Func<TEntity, object>>[] Includes);
        Task CreateAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task CreateRangeAsync(IEnumerable<TEntity> entities);
        void UpdateRange(IEnumerable<TEntity> entities);
        void DeleteRange(IEnumerable<TEntity> entities);
        int CountEntities();
        Task<string> GenerateId();
    }
}
