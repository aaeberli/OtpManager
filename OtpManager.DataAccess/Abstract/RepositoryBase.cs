using OtpManager.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OtpManager.DataAccess.Abstract
{

    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private IDataAccessAdapter _dataAccessAdapter;

        public RepositoryBase(IDataAccessAdapter dataAccessAdapter)
        {
            _dataAccessAdapter = dataAccessAdapter;
            if (_dataAccessAdapter == null) throw new NullReferenceException("Data Access Adapter must be initialized");
        }

        public TEntity Create()
        {
            return _dataAccessAdapter.Create<TEntity>();
        }

        public virtual TEntity Add(TEntity entity)
        {
            return _dataAccessAdapter.Add(entity);
        }

        public virtual IEnumerable<TEntity> Add(IEnumerable<TEntity> entities)
        {
            return _dataAccessAdapter.Add(entities);
        }

        public virtual ICollection<TEntity> Read()
        {
            return _dataAccessAdapter.GetEntities<TEntity>().ToList();
        }

        public IEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> filter)
        {
            return _dataAccessAdapter.GetEntities<TEntity>().Where(filter.Compile());
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter)
        {
            return _dataAccessAdapter.GetEntities<TEntity>().SingleOrDefault(filter.Compile());
        }

        public virtual TEntity Remove(TEntity entity)
        {
            return _dataAccessAdapter.Remove(entity);
        }

        public virtual IEnumerable<TEntity> Remove(IEnumerable<TEntity> entities)
        {
            return _dataAccessAdapter.Remove(entities);
        }

        public virtual int SaveChanges()
        {
            return _dataAccessAdapter.SaveChanges();
        }

        public virtual void UndoChanges()
        {
            _dataAccessAdapter.UndoChanges();
        }

        public void Dispose()
        {
            _dataAccessAdapter.Dispose();
        }
    }

}
