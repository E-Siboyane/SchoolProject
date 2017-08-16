using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Modules;
using SchoolProject.WebApplication.Models.Repository;

namespace SchoolProject.WebApplication.ServiceManager {
    public class NinjectBinding : NinjectModule {
        public override void Load() {
            Bind<IPerformanceManagmentRepository>().To<PerformanceManagmentRepository>();
        }
    }
}