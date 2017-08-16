using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace SchoolProject.WebApplication.ServiceManager.Interface {
    public interface INinjectStandardModule {
        IKernel GetStandardModelule();
    }
}
