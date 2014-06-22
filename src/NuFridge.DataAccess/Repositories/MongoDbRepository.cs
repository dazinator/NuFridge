using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NuFridge.DataAccess.Connection;
using NuFridge.DataAccess.Entity;

namespace NuFridge.DataAccess.Repositories
{
    /// <summary>
    /// A MongoDB repository. Maps to a collection with the same name
    /// as type TEntity.
    /// </summary>
    /// <typeparam name="T">Entity type for this repository</typeparam>
    public class MongoDbRepository<TEntity> :
        IRepository<TEntity> where
            TEntity : IEntityBase
    {
        private MongoCollection<TEntity> collection;

        private MongoRead Context = new MongoRead();

        public MongoDbRepository()
        {
            GetCollection();
        }

        public bool Insert(TEntity entity)
        {
            entity.Id = Guid.NewGuid();
            return collection.Insert(entity).Ok;
        }

        public bool Update(TEntity entity)
        {
            if (entity.Id == null)
                return Insert(entity);

            return collection
                .Save(entity)
                    .DocumentsAffected > 0;
        }

        public bool Delete(TEntity entity)
        {
            return collection
                .Remove(Query.EQ("_id", entity.Id))
                    .DocumentsAffected > 0;
        }

        public IList<TEntity> GetAll()
        {
            return collection.FindAllAs<TEntity>().ToList();
        }

        public TEntity GetById(Guid id)
        {
            return collection.FindOneByIdAs<TEntity>(id);
        }

        public IQueryable<TEntity> Get(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return collection.FindAll().AsQueryable().Where(predicate);
        }

        #region Private Helper Methods

        private void GetCollection()
        {
            collection = Context.Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }
        #endregion
    }
}
