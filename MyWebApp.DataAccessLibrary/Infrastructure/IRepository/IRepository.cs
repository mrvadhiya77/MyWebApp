using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyWebApp.DataAccessLibrary.Infrastructure.IRepository
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get All data
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Get Data By Id
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T GetT(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// Add Data
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);

        /// <summary>
        /// Update Data
        /// </summary>
        /// <param name="entity"></param>
        //void Update(T entity);

        /// <summary>
        /// Delete Data
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// Delete group of data
        /// </summary>
        /// <param name="entities"></param>
        void DeleteRange(IEnumerable<T> entities);
    }
}
