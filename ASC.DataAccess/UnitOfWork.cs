using ASC.Model.BaseTypes;
using Microsoft.EntityFrameworkCore;

namespace ASC.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private Dictionary<string, object> _repositories;
        private DbContext _dbContext;

        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int CommitTransaction()
        {
            return _dbContext.SaveChanges();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            if (_repositories == null)
                _repositories = new Dictionary<string, object>();

            var type = typeof(T).Name;
            if (_repositories.ContainsKey(type)) return (IRepository<T>)_repositories[type];
            var repositoryType = typeof(Repository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _dbContext);
            _repositories.Add(type, repositoryInstance);
            return (IRepository<T>)repositoryInstance;
        }
    }
}
