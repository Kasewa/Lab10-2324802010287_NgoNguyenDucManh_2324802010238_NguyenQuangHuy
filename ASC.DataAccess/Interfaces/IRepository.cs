using ASC.Model.BaseTypes;

namespace ASC.DataAccess
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T> FindAsync(string partitionKey, string rowKey);
        Task<IEnumerable<T>> FindAllByPartitionKeyAsync(string partitionkey);
        Task<IEnumerable<T>> FindAllAsync();
    }
}
