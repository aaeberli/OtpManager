using OtpManager.Common.Abstract;
using System.Collections.Generic;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace OtpManager.DataAccess
{
    public class DataAccessAdapter : IDataAccessAdapter
    {
        private OtpModel _dbContext;
        public DataAccessAdapter()
        {
            _dbContext = new OtpModel();
        }

        public IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : BaseEntity
        {
            return _dbContext.Set<TEntity>();
        }

        public TEntity Create<TEntity>() where TEntity : BaseEntity
        {
            return _dbContext.Set<TEntity>().Create();
        }

        public TEntity Add<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            return _dbContext.Set<TEntity>().Add(entity);
        }
        public IEnumerable<TEntity> Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            return _dbContext.Set<TEntity>().AddRange(entities);
        }

        public TEntity Remove<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            return _dbContext.Set<TEntity>().Remove(entity);
        }

        public IEnumerable<TEntity> Remove<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            return _dbContext.Set<TEntity>().RemoveRange(entities);
        }

        public int SaveChanges()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _UndoChanges();
                throw ex;
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public void UndoChanges()
        {
            _UndoChanges();
        }

        private void _UndoChanges()
        {
            foreach (DbEntityEntry entry in _dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    // Under the covers, changing the state of an entity from  
                    // Modified to Unchanged first sets the values of all  
                    // properties to the original values that were read from  
                    // the database when it was queried, and then marks the  
                    // entity as Unchanged. This will also reject changes to  
                    // FK relationships since the original value of the FK  
                    // will be restored. 
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    // If the EntityState is the Deleted, reload the date from the database.   
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
