using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PeBank.API.Contracts
{
    public interface IRepositoryBase<T>
    {
        void Create(T entity);
        void CreateMany(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteMany(IEnumerable<T> entities);
        bool Exists(object id);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression, int? skip = null, int? take = null, IEnumerable<string> includes = null);
        IEnumerable<T> FindAll(IEnumerable<string> includes = null);
        T FindById(int id, IEnumerable<string> includes = null);
        T FindSingle(Expression<Func<T, bool>> expression, IEnumerable<string> includes = null);
        void Save();
        void Update(T entity);
    }
}
