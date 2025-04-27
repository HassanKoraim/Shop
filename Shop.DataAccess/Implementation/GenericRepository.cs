using Microsoft.EntityFrameworkCore;
using Shop.DataAccess.Data;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shop.DataAccess.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;    
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, string? includeWord)
        {
            IQueryable<T> query = _dbSet;
            if(predicate != null)
            {
                query = query.Where(predicate);
            }
            if(includeWord != null)
            {
                foreach(var word in includeWord.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(word);
                }
            } 
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>>? predicate = null, string? includeWord = null)
        {
            IQueryable<T> query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeWord != null)
            {
                foreach (var word in includeWord.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(word);
                }
            }
            return query.SingleOrDefault();
        }

        public void Remove(T entity)
        {
            if (entity != null)
            {
                 _dbSet.Remove(entity);
            }
        }
        public void Remove(Expression<Func<T, bool>> predicate)
        {
            var entity = GetFirstOrDefault(predicate);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public void RemoveRange(IEnumerable<T> entity)
        {
            throw new NotImplementedException();
        }
    }
}
