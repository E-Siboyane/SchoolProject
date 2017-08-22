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

        public PMReviewPeriod AddPerformanceReviewPeriod(PMReviewPeriod performanceReviewPeriod) {
            return _pmRepository.Insert(performanceReviewPeriod,_pmRepository.GetApplicationDbContext);
        }

        public bool DeletePerformanceReviewPeriod(PMReviewPeriod performanceReviewPeriod) {
             _pmRepository.Delete(performanceReviewPeriod, _pmRepository.GetApplicationDbContext);
            return true;
        }


        public PMReviewPeriod FindPerformanceReviewPeriod(int id) {
            return _pmRepository.Find<PMReviewPeriod>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<PMReviewPeriod> GetPerformanceReviewPeriods() {
            return _pmRepository.Get<PMReviewPeriod>(_pmRepository.GetApplicationDbContext).ToList();
        }

        public PMReviewPeriod UpdatePerformanceReviewPeriod(PMReviewPeriod performanceReviewPeriod) {
            return _pmRepository.Update(performanceReviewPeriod, _pmRepository.GetApplicationDbContext);
        }

        public PMReview AddPerformanceReview(PMReview performanceReview) {
            return _pmRepository.Insert(performanceReview, _pmRepository.GetApplicationDbContext);
        }

        public PMReview UpdatePerformanceReview(PMReview performanceReview) {
            return _pmRepository.Update(performanceReview, _pmRepository.GetApplicationDbContext);
        }

        public bool DeletePerformanceReview(PMReview performanceReview) {
             _pmRepository.Delete(performanceReview, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public PMReview FindPerformanceReview(int id) {
            return _pmRepository.Find<PMReview>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<PMReview> GetPerformanceReviews() {
            return _pmRepository.Get<PMReview>(_pmRepository.GetApplicationDbContext).ToList();
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
