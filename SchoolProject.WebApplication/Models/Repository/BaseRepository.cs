using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using EntityFramework.BulkInsert.Extensions;

namespace SchoolProject.WebApplication.Models.Repository {
    public class BaseRepository :IBaseRepository{
        /// <summary>
        /// Insert new object
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="item">Class instance</param>
        /// <param name="_dbContext">DbContext</param>
        /// <returns>Class Object</returns>
        public T Insert<T>(T item, DbContext _dbContext) where T : class {
            _dbContext.Set<T>().Add(item);
            CommitTransaction(_dbContext);
            return (item);
        }

        /// <summary>
        /// Bulk insert objects. this method uses EntityFramework.BulkInsert.Extensions library
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="_dbContext">DbContext</param>
        /// <param name="bulkInsertItems">List of objects to insert</param>
        /// <returns>true</returns>
        /// 
        public bool BulkInsert<T>(DbContext _dbContext, List<T> bulkInsertItems) {
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
            _dbContext.Configuration.ValidateOnSaveEnabled = false;
            var transactionScope = _dbContext.Database.BeginTransaction();
            _dbContext.BulkInsert(bulkInsertItems);
            _dbContext.SaveChanges();
            transactionScope.Commit();
            _dbContext.Configuration.AutoDetectChangesEnabled = true;
            _dbContext.Configuration.ValidateOnSaveEnabled = true;
            return true;
        }

        /// <summary>
        /// Delete object from repository
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="item">Class Instance/Object</param>
        /// <param name="_dbContext">DbContext</param>
        /// <returns>Class Instance/Object</returns>
        public T Delete<T>(T item, DbContext _dbContext) where T : class {
            _dbContext.Set<T>().Remove(item);
            CommitTransaction(_dbContext);
            return (item);
        }

        /// <summary>
        /// Update item in repository
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="item">Class Instance/Object</param>
        /// <param name="_dbContext">DbContext</param>
        /// <returns>Class Instance/Object</returns>
        public T Update<T>(T item, DbContext _dbContext) where T : class {
            DbEntityEntry entry = _dbContext.Entry<T>(item);
            if (entry.State == EntityState.Detached) {
                entry.State = EntityState.Modified;
                CommitTransaction(_dbContext);
            }
            return (item);
        }

        /// <summary>
        /// Search for an item in the repository
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="id">Object Id (1)</param>
        /// <param name="_dbContext">DbContext</param>
        /// <returns></returns>
        public T Find<T>(int id, DbContext _dbContext) where T : class {
            if (id > 0) {
                var item = _dbContext.Set<T>().Find(id);
                return (item);
            }
            return null;
        }

        /// <summary>
        /// Get list of objects that meet the criteria
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="_dbContext">DbContext</param>
        /// <returns></returns>
        public IQueryable<T> Get<T>(DbContext _dbContext) where T : class {
            return (_dbContext.Set<T>().AsQueryable());
        }

        /// <summary>
        /// Persist single change to the store (DB) from Object Graph
        /// </summary>
        /// <param name="_dbcontext">DbContext</param>
        /// <returns>Object Id from Data Store</returns>
        public int Commit(DbContext _dbcontext) {
            return (_dbcontext.SaveChanges());
        }

        /// <summary>
        /// Persist multiple changes to the store (DB) from Object Grapth
        /// </summary>
        /// <param name="_dbcontext">DbContext</param>
        /// <returns>true/false</returns>
        public bool CommitTransaction(DbContext _dbcontext) {
            return (_dbcontext.SaveChanges() > 0 ? true : false);
        }
    }
}