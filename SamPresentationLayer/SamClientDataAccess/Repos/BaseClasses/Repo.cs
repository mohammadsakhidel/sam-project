using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamClientDataAccess.Repos.BaseClasses
{
    public abstract class Repo<C, T> : IDisposable
        where C : DbContext
        where T : class
    {
        #region Fields:
        protected C context;
        protected DbSet<T> set;
        #endregion

        #region Props:
        public C Context
        {
            get
            {
                return context;
            }
        }
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
        public virtual void Add(T entity)
        {
            set.Add(entity);
        }

        public virtual void AddWithSave(T entity)
        {
            Add(entity);
            Save();
        }

        public virtual int Count()
        {
            return set.Count();
        }

        public virtual T Get(params object[] id)
        {
            return set.Find(id);
        }

        public virtual bool Exists(params object[] id)
        {
            return set.Find(id) != null;
        }

        public virtual List<T> GetAll()
        {
            return set.ToList();
        }

        public virtual void Remove(params object[] id)
        {
            var entity = Get(id);
            Remove(entity);
        }

        public virtual void Remove(T entity)
        {
            set.Remove(entity);
        }

        public virtual void RemoveWithSave(params object[] id)
        {
            Remove(id);
            Save();
        }

        public virtual void RemoveWithSave(T entity)
        {
            Remove(entity);
            Save();
        }

        public virtual void Save()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public void SetLogAction(Action<string> action)
        {
            this.context.Database.Log = action;
        }
        #endregion
    }
}
