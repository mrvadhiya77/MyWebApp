using Microsoft.EntityFrameworkCore;
using MyWebApp.DataAccesLayer.Data;
using MyWebApp.DataAccessLibrary.Infrastructure.IRepository;
using System.Linq.Expressions;

namespace MyWebApp.DataAccessLibrary.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MyWebAppContext _context;

        private DbSet<T> _dbSet;

        public Repository(MyWebAppContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public T GetT(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).FirstOrDefault();
        }
    }
}
