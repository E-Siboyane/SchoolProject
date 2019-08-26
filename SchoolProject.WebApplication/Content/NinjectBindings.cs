using EconomicCapital.RF.DataAccess;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.BusinessLogic
{
    public class NinjectBindings :  NinjectModule
    {
        public override void Load() {
            Bind<ISimPakDataProcessing>().To<SimPakDataProcessing>();
            Bind<ApplicationDbContext>().ToSelf();
            Bind<RFDatabaseContext>().ToSelf();
            Bind<IRFDataProcessing>().To<RFDataProcessing>();
            Bind<IFileManager>().To<FileManager>();
            Bind<IRetailPoolingManager>().To<RetailPoolingManager>();
        }
    }
}
