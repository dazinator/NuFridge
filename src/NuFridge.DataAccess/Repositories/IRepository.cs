using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NuFridge.DataAccess.Entity;

namespace NuFridge.DataAccess.Repositories
{
    public interface IRepository<TEntity> where TEntity : IEntityBase
    {
        bool Insert(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(TEntity entity);
        IList<TEntity> GetAll();
        TEntity GetById(Guid id);
    }
}
