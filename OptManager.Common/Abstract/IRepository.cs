using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OtpManager.Common.Abstract
{
    public interface IRepository<TEntity> : IDisposable where TEntity : BaseEntity
    {
        TEntity Create();
        TEntity Add(TEntity entity);
        IEnumerable<TEntity> Add(IEnumerable<TEntity> entities);
        ICollection<TEntity> Read();
        IEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> filter);
        TEntity Remove(TEntity entity);
        IEnumerable<TEntity> Remove(IEnumerable<TEntity> entities);
        int SaveChanges();
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter);
        void UndoChanges();
    }

}
