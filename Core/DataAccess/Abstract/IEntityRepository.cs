using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities.Abstract;

namespace Core.DataAccess.Abstract
{
    public interface IEntityRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAll(Expression<Func<T, bool>> filter = null);

        Task<T?> Get(Expression<Func<T, bool>> filter);
        Task Add(T entity);
        Task Delete(T entity);
        Task Update(T? entity);

    }
}
