using OtpManager.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OtpManager.DataAccess.Abstract
{

    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        public IDataAccessAdapter DataAccessAdapter { get; private set; }

        public RepositoryBase(IDataAccessAdapter unitOfWorkAdapter)
        {
            DataAccessAdapter = unitOfWorkAdapter;
        }
        public TEntity Create()
        {
            return DataAccessAdapter.Create<TEntity>();
        }

        public virtual TEntity Add(TEntity entity)
        {
            return DataAccessAdapter.Add(entity);
        }

        public virtual IEnumerable<TEntity> Add(IEnumerable<TEntity> entities)
        {
            return DataAccessAdapter.Add(entities);
        }

        public virtual ICollection<TEntity> Read()
        {
            return DataAccessAdapter.GetEntities<TEntity>().ToList();
        }

        public IEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> filter)
        {
            return DataAccessAdapter.GetEntities<TEntity>().Where(filter.Compile());
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter)
        {
            return DataAccessAdapter.GetEntities<TEntity>().SingleOrDefault(filter.Compile());
        }

        public virtual TEntity Remove(TEntity entity)
        {
            return DataAccessAdapter.Remove(entity);
        }

        public virtual IEnumerable<TEntity> Remove(IEnumerable<TEntity> entities)
        {
            return DataAccessAdapter.Remove(entities);
        }

        public virtual int SaveChanges()
        {
            return DataAccessAdapter.SaveChanges();
        }

        public virtual void UndoChanges()
        {
            DataAccessAdapter.UndoChanges();
        }

        public void Dispose()
        {
            DataAccessAdapter.Dispose();
        }
    }

}
