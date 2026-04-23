using ASC.Model.BaseTypes;
using Microsoft.EntityFrameworkCore;

namespace ASC.DataAccess
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DbContext _dbContext;
        private DbSet<T> _dbSet;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<T> FindAsync(string partitionKey, string rowKey)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.PartitionKey == partitionKey && e.RowKey == rowKey);
        }

        public async Task<IEnumerable<T>> FindAllByPartitionKeyAsync(string partitionkey)
        {
            return await _dbSet.Where(e => e.PartitionKey == partitionkey).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
