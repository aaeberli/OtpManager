using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtpManager.Common.Abstract
{
    public interface IDataAccessAdapter : IDisposable
    {
        IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : BaseEntity;

        TEntity Create<TEntity>() where TEntity : BaseEntity;

        TEntity Add<TEntity>(TEntity entity) where TEntity : BaseEntity;

        IEnumerable<TEntity> Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

        TEntity Remove<TEntity>(TEntity entity) where TEntity : BaseEntity;

        IEnumerable<TEntity> Remove<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

        int SaveChanges();

        void UndoChanges();
    }
}
