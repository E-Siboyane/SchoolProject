[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SchoolProject.WebApplication.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(SchoolProject.WebApplication.App_Start.NinjectWebCommon), "Stop")]

namespace SchoolProject.WebApplication.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Models.Repository;
    using Ninject;
    using Ninject.Web.Common;
    using ServiceManager;
    using ServiceManager.Interface;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                kernel.Bind<IBaseRepository>().To<BaseRepository>();
                kernel.Bind<IPerformanceManagmentRepository>().To<PerformanceManagmentRepository>();
                kernel.Bind<IAdmininstrationManager>().To<AdmininstrationManager>();
                kernel.Bind<INinjectStandardModule>().To<NinjectStandardModuleManager>();
                kernel.Bind<IPerformanceDocumentManager>().To<PerformanceDocumentManager>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
        }        
    }
}
