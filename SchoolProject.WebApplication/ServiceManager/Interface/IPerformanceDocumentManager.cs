using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolProject.WebApplication.DTO;

namespace SchoolProject.WebApplication.ServiceManager.Interface {
    public interface IPerformanceDocumentManager {
        List<LinkPerformanceYearReviewPeriod> GetLinkedPerformanceYearReviews();
    }
}
