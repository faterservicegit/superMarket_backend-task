using Microsoft.EntityFrameworkCore;
using FaterRasanehMarket.Common;
using FaterRasanehMarket.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FaterRasanehMarket.Data.Repositories
{
    class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity> where TEntity : class where TContext : DbContext
    {
        protected TContext _Context { get; set; }
        private readonly DbSet<TEntity> dbSet;
        public BaseRepository(TContext context)
        {
            _Context = context;
            _Context.CheckArgumentIsNull(nameof(_Context));

            dbSet = _Context.Set<TEntity>();
            dbSet.CheckArgumentIsNull(nameof(dbSet));
        }
        public async Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return await dbSet.AsNoTracking().ToListAsync();
        }
        public IEnumerable<TEntity> FindAll()
        {
            return dbSet.AsNoTracking().ToList();
        }
        public async Task<TEntity> FindByIdAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }
        public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderdBy = null, params Expression<Func<TEntity, object>>[] Includes)
        {
            IQueryable<TEntity> query = _Context.Set<TEntity>();
            foreach (var inculde in Includes)
            {
                query = query.Include(inculde);
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (OrderdBy != null)
            {
                query = OrderdBy(query);
            }
            return await query.ToListAsync();
        }
        public async Task CreateAsync(TEntity entity) => await dbSet.AddAsync(entity);
        public void Update(TEntity entity) => dbSet.Update(entity);
        public void Delete(TEntity entity) => dbSet.Remove(entity);
        public async Task CreateRangeAsync(IEnumerable<TEntity> entities) => await dbSet.AddRangeAsync(entities);
        public void UpdateRange(IEnumerable<TEntity> entities) => dbSet.UpdateRange(entities);
        public void DeleteRange(IEnumerable<TEntity> entities) => dbSet.RemoveRange(entities);
        public async Task<List<TEntity>> GetPaginateResultAsync(int currentPage, int pagesize = 1)
        {
            var entites = await FindAllAsync();
            return entites.Skip((currentPage - 1) * pagesize).Take(pagesize).ToList();
        }
        public int CountEntities() => dbSet.Count();
        public async Task<string> GenerateId()
        {
            var Id = StringExtensions.GenerateId(10);
            var entity = await dbSet.FindAsync(Id);
            while (entity != null)
            {
                Id = StringExtensions.GenerateId(10);
                entity = await dbSet.FindAsync(Id);
            }
            return Id;
        }
    }
}
