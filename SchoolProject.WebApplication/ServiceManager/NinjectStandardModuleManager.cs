using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using SchoolProject.WebApplication.ServiceManager.Interface;

namespace SchoolProject.WebApplication.ServiceManager {
    public class NinjectStandardModuleManager : INinjectStandardModule {
        public IKernel GetStandardModelule() {
            IKernel kernel = new StandardKernel(new NinjectBinding());
            //var modules = new List<INinjectModule>() {
            //    new RepositoryBinding(),
            //    new CommonBinding()
            //};
            //kernel.Load(modules);
            return kernel;
        }
    }
}
