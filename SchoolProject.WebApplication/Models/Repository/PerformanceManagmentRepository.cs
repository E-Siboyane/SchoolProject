using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.Models.Repository {
    /// <summary>
    /// Implementation of PerformanceManagement Repository, it inherits from the BaseRepository for common database CRUD Operations
    /// </summary>
    public class PerformanceManagmentRepository : BaseRepository, IPerformanceManagmentRepository, IDisposable {
        private readonly ApplicationDatabaseContext _applicationDbContext;
        public PerformanceManagmentRepository(ApplicationDatabaseContext _context) {
            _applicationDbContext = _context;
        }
        /// <summary>
        /// Return Application Database DbContext
        /// </summary>
        public ApplicationDatabaseContext GetApplicationDbContext { get { return _applicationDbContext; } }

        /// <summary>
        /// Dispose the Context after use
        /// </summary>
        public void Dispose() {
            _applicationDbContext.Dispose();
        }
    }
}