using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.WebApplication.Models.Repository {
    public interface IBaseRepository {
        T Insert<T>(T item, DbContext _dbContext) where T : class;
        bool BulkInsert<T>(DbContext _dbContext, List<T> bulkInsertItems);
        T Delete<T>(T item, DbContext _dbContext) where T : class;
        T Update<T>(T item, DbContext _dbContext) where T : class;
        T Find<T>(int id, DbContext _dbContext) where T : class;
        IQueryable<T> Get<T>(DbContext _dbContext) where T : class;
        int Commit(DbContext _dbcontext);
        bool CommitTransaction(DbContext _dbcontext);
    }
}
