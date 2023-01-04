using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.RegularExpressions;

namespace Infrastructure.Repositories.Base
{
    public class RepositoryBase: IRepositoryBase
    {
        private static readonly log4net.ILog Logging = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ApplyMigration(string migrationName)
        {
            var context = new TemplateDbContext();
            var migrator = context.GetInfrastructure().GetService<IMigrator>();
            migrator.Migrate(migrationName);
        }

        public Response<TEntity> Get<TEntity>(
            string userId,
            IEnumerable<Filter> filters = null,
            IEnumerable<string> includes = null,
            bool showDeleted = false)
            where TEntity : class, IDbItem
        {
            List<Filter> filterList = new();
            if (filters != null)
                filterList = filters.ToList();

            filterList.Add(Filter.ByUserId(userId));
            return ForceGet<TEntity>(filterList, includes, showDeleted);
        }
        public Response<TEntity> ForceGet<TEntity>(
            IEnumerable<Filter> filters = null,
            IEnumerable<string> includes = null,
            bool showDeleted = false)
            where TEntity : class, IDbItem
        {
            try
            {
                List<Filter> filterList = new();
                if (filters != null)
                    filterList = filters.ToList();
                else
                    filterList = new List<Filter>();

                if (!showDeleted)
                    filterList.Add(Filter.Null("Deleted"));

                var result = Filtering<TEntity>(filterList, includes);

                return new Response<TEntity>(true, result,
                    $"[OK] Get - {typeof(TEntity).Name} - filters: {filters != null} - includes: {includes != null} - showDeleted: {showDeleted}",
                    HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Logging.Error($"[ERROR] Get - {typeof(TEntity).Name} - filters: {filters != null} - includes: {includes != null} - showDeleted: {showDeleted}");
                Logging.Error(ex);
                return new Response<TEntity>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        
        public Response<TEntity> Add<TEntity>(TEntity entity)
            where TEntity : class, IDbItem
        {
            try
            {
                var _context = new TemplateDbContext();
                entity = _context.Add(entity).Entity;
                if (_context.SaveChanges() > 0)
                    return new Response<TEntity>(true, new List<TEntity>() { entity },
                        $"[OK] Add - {typeof(TEntity).Name} - id: {entity.Id}",
                        HttpStatusCode.Created);
                else
                    return new Response<TEntity>(false, 0, $"No item added", HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                Logging.Error($"[ERROR] Add - {typeof(TEntity).Name}");
                Logging.Error(ex);
                return new Response<TEntity>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response<TEntity> Update<TEntity>(TEntity entity, string userId)
            where TEntity : class, IDbItem
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userId) && ValidateUserId<TEntity>(entity.Id, userId))
                {
                    entity.UserId = userId;
                    return ForceUpdate(entity);
                }
                else
                    return new Response<TEntity>(false, 0, $"Cannot verify if you are the owner of this {typeof(TEntity).Name} item", HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                Logging.Error($"[ERROR] Update - {typeof(TEntity).Name}");
                Logging.Error(ex);
                return new Response<TEntity>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response<TEntity> ForceUpdate<TEntity>(TEntity entity)
            where TEntity : class, IDbItem
        {
            try
            {
                var _context = new TemplateDbContext();
                entity = _context.Update(entity).Entity;
                if (_context.SaveChanges() > 0)
                {
                    var itemSearched = _context.Set<TEntity>().Find(entity.Id);
                    return new Response<TEntity>(true, new List<TEntity>() { itemSearched },
                        $"[OK] Update - {typeof(TEntity).Name} - id: {entity.Id}", HttpStatusCode.Accepted);
                }
                else
                    return new Response<TEntity>(false, 0, "There are no changes detected", HttpStatusCode.InternalServerError);
                    
            }
            catch (Exception ex)
            {
                Logging.Error($"[ERROR] Update - {typeof(TEntity).Name}");
                Logging.Error(ex);
                return new Response<TEntity>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response Remove<TEntity>(int id, string userId, bool softDelete = true) 
            where TEntity : class, IDbItem
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userId) && ValidateUserId<TEntity>(id, userId))
                {
                    var _context = new TemplateDbContext();
                    var entity = _context.Set<TEntity>().Find(id);
                    if (entity != null)
                    {
                        if (softDelete)
                        {
                            entity.UserId = userId;
                            entity.Deleted = DateTime.Now;
                            entity = _context.Update(entity).Entity;
                            if (_context.SaveChanges() > 0)
                            {
                                var itemSearched = _context.Set<TEntity>().Find(entity.Id);
                                return new Response<TEntity>(true, 1,
                                    $"[OK] Remove - {typeof(TEntity).Name} - id: {id} - softDelete: {softDelete}", HttpStatusCode.OK);
                            }
                            else
                                return new Response<TEntity>(false, 0, "Could not do a soft delete", HttpStatusCode.InternalServerError);
                        }
                        else
                        {
                            _context.Remove(entity);
                            if (_context.SaveChanges() > 0)
                                return new Response(true, 1, 
                                    $"[OK] Remove - {typeof(TEntity).Name} - id: {id} - softDelete: {softDelete}", HttpStatusCode.NoContent);
                            else
                                return new Response(false, 0, "Item could not be deleted", HttpStatusCode.InternalServerError);
                        }
                    }
                    else
                        return new Response(false, 0, $"{typeof(TEntity).Name} with this id, not found", HttpStatusCode.NotFound);
                }
                else
                    return new Response<TEntity>(false, 0, $"Cannot verify if you are the owner of this {typeof(TEntity).Name} item", HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                Logging.Error($"[ERROR] Remove - {typeof(TEntity).Name} - id: {id} - softDelete: {softDelete}");
                Logging.Error(ex);
                return new Response(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        #region Internal functions
        private bool ValidateUserId<TEntity>(int entityId, string userId) where TEntity : class, IDbItem
        {
            var _context = new TemplateDbContext();
            var result = _context.Set<TEntity>().Where(x => x.Id == entityId && x.UserId == userId).Any();
            _context.Dispose();
            return result;
        }
        private IEnumerable<TEntity> Filtering<TEntity>(IEnumerable<Filter> filters, IEnumerable<string> includes) where TEntity : class, IDbItem
        {
            var _context = new TemplateDbContext();
            IEnumerable<TEntity> result = Includes(_context.Set<TEntity>(), includes);
            if (filters != null && filters.Count() > 0)
                foreach (var filter in filters.Where(x => x.Value != null))
                    result = result.Where(x => ApplyFilter(x, filter));
            return result;
        }
        public static IQueryable<T> Includes<T>(IQueryable<T> queryable, IEnumerable<string> includes) where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            if(includes != null)
            {
                foreach (var includeString in includes)
                {
                    if (properties.Any(x => x.Name == includeString))
                    {
                        queryable = queryable.Include(includeString).IgnoreAutoIncludes();
                    }
                }
            }
            
            return queryable;
        }
        public bool ApplyFilter<TEntity>(TEntity x, Filter filter)
        {
            //var prop = x.GetType().GetProperty(char.ToUpper(filter.Key[0]) + filter.Key[1..]);
            var prop = x.GetType().GetProperty(filter.Key);
            if(prop != null && filter.IsDateTime)
            {
                return filter.FilterType switch
                {
                    FilterType.Contains => prop.GetValue(x) != null && (DateTime)prop.GetValue(x) > (DateTime)filter.Value,
                    FilterType.LessThan => prop.GetValue(x) != null && (DateTime)prop.GetValue(x) < (DateTime)filter.Value,
                    FilterType.LessThanOrEqual => prop.GetValue(x) != null && (DateTime)prop.GetValue(x) <= (DateTime)filter.Value,
                    FilterType.GreaterThan => prop.GetValue(x) != null && (DateTime)prop.GetValue(x) > (DateTime)filter.Value,
                    FilterType.GreaterThanOrEqual => prop.GetValue(x) != null && (DateTime)prop.GetValue(x) >= (DateTime)filter.Value,
                    FilterType.Equal => prop.GetValue(x) != null && (DateTime)prop.GetValue(x) == (DateTime)filter.Value,
                    FilterType.Null => prop.GetValue(x) == null,
                    FilterType.NotNull => prop.GetValue(x) != null,
                    _ => false,
                };
            }
            else
            {
                return filter.FilterType switch
                {
                    FilterType.Contains => prop.GetValue(x) != null && prop.GetValue(x).ToString().Contains(filter.Value.ToString()),
                    FilterType.LessThan => prop.GetValue(x) != null && Convert.ToDouble(prop.GetValue(x)) < Convert.ToDouble(filter.Value),
                    FilterType.LessThanOrEqual => prop.GetValue(x) != null && Convert.ToDouble(prop.GetValue(x)) <= Convert.ToDouble(filter.Value),
                    FilterType.GreaterThan => prop.GetValue(x) != null && Convert.ToDouble(prop.GetValue(x)) > Convert.ToDouble(filter.Value),
                    FilterType.GreaterThanOrEqual => prop.GetValue(x) != null && Convert.ToDouble(prop.GetValue(x)) >= Convert.ToDouble(filter.Value),
                    FilterType.Equal => prop.GetValue(x) != null && prop.GetValue(x).ToString() == filter.Value.ToString(),
                    FilterType.Null => prop.GetValue(x) == null,
                    FilterType.NotNull => prop.GetValue(x) != null,
                    _ => false,
                };
            }
        }
        public static string ToSnakeCase(string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
                return Regex.Replace(s[0].ToString().ToLower() + s[1..], "[A-Z]", "_$0").ToLower();
            else
                return s;
        }
        public static string ToCamelCase(string s1)
        {
            // return Regex.Replace(s1,).ToLower();
            return Regex.Replace(s1, "_[a-z]", delegate (Match m) {
                return m.ToString().TrimStart('_').ToUpper();
            });
        }
        #endregion


    }
}