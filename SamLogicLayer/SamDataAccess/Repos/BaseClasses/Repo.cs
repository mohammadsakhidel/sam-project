using SamDataAccess.Repos.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.BaseClasses
{
    public abstract class Repo<C, T> : IRepo<C, T>
        where C : DbContext
        where T : class
    {
        #region Fields:
        DbContext context;
        DbSet<T> set;
        #endregion

        #region CTors:
        public Repo()
        {
            context = Activator.CreateInstance<C>();
            set = context.Set<T>();
        }
        public Repo(C context)
        {
            this.context = context;
            set = this.context.Set<T>();
        }
        #endregion

        #region IRepo Implementation:
        public void Add(T entity)
        {
            set.Add(entity);
        }

        public void AddWithSave(T entity)
        {
            Add(entity);
            Save();
        }

        public int Count()
        {
            return set.Count();
        }

        public T Get(params object[] id)
        {
            return set.Find(id);
        }

        public List<T> GetAll()
        {
            return set.ToList();
        }

        public void Remove(params object[] id)
        {
            var entity = Get(id);
            Remove(entity);
        }

        public void Remove(T entity)
        {
            set.Remove(entity);
        }

        public void RemoveWithSave(params object[] id)
        {
            Remove(id);
            Save();
        }

        public void RemoveWithSave(T entity)
        {
            Remove(entity);
            Save();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
        #endregion
    }
}
