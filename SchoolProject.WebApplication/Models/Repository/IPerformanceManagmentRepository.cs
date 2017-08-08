using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.WebApplication.Models.Repository {
   public interface IPerformanceManagmentRepository : IBaseRepository {
        ApplicationDatabaseContext GetApplicationDbContext { get; }
    }
}
