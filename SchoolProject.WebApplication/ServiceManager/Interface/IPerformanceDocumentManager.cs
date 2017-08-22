using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolProject.WebApplication.DTO;
using SchoolProject.WebApplication.Models;

namespace SchoolProject.WebApplication.ServiceManager.Interface {
    public interface IPerformanceDocumentManager {
        PMReviewPeriod AddPerformanceReviewPeriod(PMReviewPeriod performanceReviewPeriod);

        bool DeletePerformanceReviewPeriod(PMReviewPeriod performanceReviewPeriod);

        PMReviewPeriod FindPerformanceReviewPeriod(int id);

        List<PMReviewPeriod> GetPerformanceReviewPeriods();

        PMReviewPeriod UpdatePerformanceReviewPeriod(PMReviewPeriod performanceReviewPeriod);

        PMReview AddPerformanceReview(PMReview performanceReview);

        PMReview UpdatePerformanceReview(PMReview performanceReview);

        bool DeletePerformanceReview(PMReview performanceReview);

        PMReview FindPerformanceReview(int id);

        List<PMReview> GetPerformanceReviews();

        List<LinkPerformanceYearReviewPeriod> GetLinkedPerformanceYearReviews();
    }
}
