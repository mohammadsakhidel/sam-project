using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDataAccess.Repos.Interfaces
{
    public interface IRepo<C, T> : IDisposable
        where C : DbContext
        where T : class
    {
        #region Queries:
        T Get(params object[] id);
        List<T> GetAll();
        int Count();
        void Add(T entity);
        void AddWithSave(T entity);
        void Remove(T entity);
        void Remove(params object[] id);
        void RemoveWithSave(T entity);
        void RemoveWithSave(params object[] id);
        void Save();
        #endregion

        #region Methods:
        void SetLogAction(Action<string> action);
        #endregion
    }
}
