using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.BusinessLogic {
    public interface INinjectKernel {
        IKernel GetNinjectBindings();
    }
}
