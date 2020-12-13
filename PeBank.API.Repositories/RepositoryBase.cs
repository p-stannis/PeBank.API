using Microsoft.EntityFrameworkCore;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PeBank.API.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
    {
        #region Properties
        protected readonly DbContext DbContext;
        protected readonly DbSet<T> DbSet;
        #endregion

        #region Constructors
        protected RepositoryBase(DbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = dbContext.Set<T>();
        }
        #endregion

        #region IRepositoryBase methods
        public void Create(T entity)
        {
            DbContext.Set<T>().Add(entity);
        }

        public void CreateMany(IEnumerable<T> entities)
        {
            DbContext.Set<T>().AddRange(entities);
        }

        public void Delete(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }

        public void DeleteMany(IEnumerable<T> entities)
        {
            DbContext.Set<T>().RemoveRange(entities);
        }

        public bool Exists(object id)
        {
            var entity = DbContext.Set<T>().Find(id);
            return entity != null;
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression, int? skip = null, int? take = null, IEnumerable<string> includes = null)
        {
            var query = DbContext.Set<T>().Where(expression);

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return LoadDependencies(query, includes);
        }

        public IEnumerable<T> FindAll(IEnumerable<string> includes = null)
        {
            var query = DbContext.Set<T>().Where(x => true);

            return LoadDependencies(query, includes);
        }

        public T FindById(int id, IEnumerable<string> includes = null)
        {
            return FindSingle(entity => entity.Id == id, includes);
        }

        public T FindSingle(Expression<Func<T, bool>> expression, IEnumerable<string> includes = null)
        {
            return Find(expression, includes: includes).FirstOrDefault();
        }

        public void Save()
        {
            DbContext.SaveChanges();
        }

        public void Update(T entity)
        {
            DbContext.Set<T>().Update(entity);
        }
        #endregion

        #region Private methods
        private IQueryable<T> LoadDependencies(IQueryable<T> query, IEnumerable<string> includes)
        {
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }
        #endregion
    }
}
