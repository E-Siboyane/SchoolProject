using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.BusinessLogic {
    public class NinjectKernel : INinjectKernel {
        public IKernel GetNinjectBindings() {
           var kernel =  new StandardKernel(new NinjectBindings());
           return (kernel);
        }
    }
}
