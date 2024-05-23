using ApplicationTrackingSystem.Data;
using ApplicationTrackingSystem.DataAccess.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApplicationTrackingSystem.DataAccess.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> DbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            DbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await DbSet.AddAsync(entity).ConfigureAwait(false);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (filter != null)
                query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var includeprop in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);
                }
            }
            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (filter != null)
                query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            if (orderBy != null)
                return await orderBy(query).ToListAsync().ConfigureAwait(false);
            return await query.ToListAsync().ConfigureAwait(false);
        }

        public void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbSet.Remove(entity);
        }

        public void Remove(int id)
        {
            T entityToRemove = DbSet.Find(id);
            if (entityToRemove != null)
                DbSet.Remove(entityToRemove);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            DbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public T Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public T FirstorDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            throw new NotImplementedException();
        }
        // Method implementations removed, as they are not needed and were throwing exceptions.
    }
}
