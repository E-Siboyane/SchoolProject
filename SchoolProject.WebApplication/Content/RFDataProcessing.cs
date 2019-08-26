using EconomicCapital.RF.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
    public class RFDataProcessing : IRFDataProcessing, IDisposable {
        private readonly RFDatabaseContext _dbContextRFStaging;

        public RFDataProcessing(RFDatabaseContext dbRFStaging) {
            _dbContextRFStaging = dbRFStaging;
        }

        public RFDatabaseContext GetRFStagingDbContext() {
            return _dbContextRFStaging;
        }

        public string AddJob(DTO.Job jobDetails) {
            var result = string.Empty;
            try {
                _dbContextRFStaging.Jobs.Add(jobDetails);
                _dbContextRFStaging.SaveChanges();
                return jobDetails.JobId.ToString();
            }
            catch (DbEntityValidationException e) {
               result = string.Empty;
                foreach (var eve in e.EntityValidationErrors) {
                    foreach (var ve in eve.ValidationErrors) {
                        result += string.Format("{0}: {1} || ",ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return result;
            }
            catch (Exception ex) {
                return ex.Message.ToString();
            }
        }

        public bool UpdateJob(int jobId) {
            var job = _dbContextRFStaging.Jobs.Find(jobId);
            job.Status = "Loading Complete";
            _dbContextRFStaging.SaveChanges();
            return (_dbContextRFStaging.SaveChanges() >= 1) ? true : false;
        }

        public bool BulkInsert<T>(List<T> bulkInsertItems, ImportType importType) where T : class {
            var conString = _dbContextRFStaging.Database.Connection.ConnectionString;
            using (var connection = new SqlConnection(conString)) {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)) {
                    bulkCopy.BatchSize = 100;
                    //var destinationTableName = bulkInsertItems.GetType().ToString().Split('.').LastOrDefault();
                    //destinationTableName = destinationTableName.Remove(destinationTableName.Length - 1);
                    bulkCopy.DestinationTableName = string.Format("[dbo].[{0}]", importType.ToString().ToLower());

                    try {
                        var data = bulkInsertItems.AsDataTable();
                        bulkCopy.WriteToServer(data);
                    }
                    catch (Exception exception) {
                        Console.WriteLine(exception.Message.ToString());
                        transaction.Rollback();
                        connection.Close();
                    }
                }
                transaction.Commit();
            }
            return true;
        }

        public void Dispose() {
            _dbContextRFStaging.Dispose();
        }
    }
}
