using Core.Interfaces;
using Core.Models;
using System.Collections.Generic;

namespace Infrastructure.Repositories.Base
{
    public interface IRepositoryBase
    {
        public void ApplyMigration(string migrationName);
        public Response<TEntity> Get<TEntity>(
            string userId,
            IEnumerable<Filter> filters = null,
            IEnumerable<string> includes = null,
            bool showDeleted = false) where TEntity : class, IDbItem;
        public Response<TEntity> ForceGet<TEntity>(
            IEnumerable<Filter> filters = null,
            IEnumerable<string> includes = null,
            bool showDeleted = false) where TEntity : class, IDbItem;
        public Response<TEntity> Add<TEntity>(TEntity entity) where TEntity : class, IDbItem;
        public Response<TEntity> Update<TEntity>(TEntity entity, string userId) where TEntity : class, IDbItem;
        public Response<TEntity> ForceUpdate<TEntity>(TEntity entity) where TEntity : class, IDbItem;
        public Response Remove<TEntity>(int id, string userId, bool softDelete = true) where TEntity : class, IDbItem;

    }
}
