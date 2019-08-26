using EconomicCapital.RF.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
    public interface IRFDataProcessing {
        bool BulkInsert<T>(List<T> bulkInsertItems, ImportType importType) where T : class;
        string AddJob(Job jobDetails);
        bool UpdateJob(int jobId);
        RFDatabaseContext GetRFStagingDbContext();
    }
}
