using OtpManager.Common.Abstract;
using System.Collections.Generic;

namespace OtpManager.DataAccess
{
    public class DataAccessAdapter : IDataAccessAdapter
    {
        private OtpModel _dbContext;
        public DataAccessAdapter()
        {
            _dbContext = new OtpModel();
        }

        public IEnumerable<T> GetEntities<T>() where T : BaseEntity
        {
            return _dbContext.Set<T>();
        }

        public T Create<T>() where T : BaseEntity
        {
            return _dbContext.Set<T>().Create();
        }

        public T Add<T>(T entity) where T : BaseEntity
        {
            return _dbContext.Set<T>().Add(entity);
        }

        public T Remove<T>(T entity) where T : BaseEntity
        {
            return _dbContext.Set<T>().Remove(entity);
        }

        public IEnumerable<T> RemoveRange<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            return _dbContext.Set<T>().RemoveRange(entities);
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

    }
}
