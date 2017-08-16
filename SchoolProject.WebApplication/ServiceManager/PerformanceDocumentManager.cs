using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Ninject;
using SchoolProject.WebApplication.DTO;
using SchoolProject.WebApplication.Models;
using SchoolProject.WebApplication.Models.Repository;
using SchoolProject.WebApplication.ServiceManager.Interface;

namespace SchoolProject.WebApplication.ServiceManager {
    public class PerformanceDocumentManager : IPerformanceDocumentManager {
        private IPerformanceManagmentRepository _pmRepository;
        private INinjectStandardModule _standardModule;

        public PerformanceDocumentManager(INinjectStandardModule ninjectStandardModules) {
            _standardModule = ninjectStandardModules;
            IKernel kernel = _standardModule.GetStandardModelule();
            _pmRepository = kernel.Get<IPerformanceManagmentRepository>();
        }

        public List<LinkPerformanceYearReviewPeriod> GetLinkedPerformanceYearReviews() {
            var results = new List<LinkPerformanceYearReviewPeriod>();
            var linkedPerformanceYears = _pmRepository.Get<PMReviewPeriod>(_pmRepository.GetApplicationDbContext).
                                         Where(x => x.DateDeleted == null && x.StatusId != 4).
                                         Include(x => x.Status).Include(x => x.PerformanceYear).Include(x => x.ReviewPeriod);
            foreach (var item in linkedPerformanceYears) {
                var review = new LinkPerformanceYearReviewPeriod() {
                    PMReviewPeriodId = item.PMReviewPeriodId,
                    ReviewPeriodId = item.ReviewPeriodId,
                    PerformanceYearId = item.PerformanceYearId,
                    PerformanceYear = item.PerformanceYear.PerformanceYearName,
                    PerformanceYearStartEndDate = string.Format("{0} - {1}",
                                                                       item.PerformanceYear.StartDate.ToString("MMMM dd yyyy"),
                                                                       item.PerformanceYear.EndDate.ToString("MMMM dd yyyy")),
                    ReviewPeriod = item.ReviewPeriod.ReviewPeriodName,
                    StatusId = item.StatusId,
                    StatusDescription = item.Status.StatusName
                };
                results.Add(review);
            }
            return (results);
        }
    }
}
