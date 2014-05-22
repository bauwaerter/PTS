using System.Web.Http.Dependencies;
using Ninject;

namespace Blizzard.Infrastructure {

    public class NinjectResolver : NinjectScope, IDependencyResolver{

        private readonly IKernel _kernel;

        public NinjectResolver(IKernel kernal) : base(kernal){
            _kernel = kernal;
        }

        public IDependencyScope BeginScope(){
            return new NinjectScope(_kernel.BeginBlock());
        }
    }
}
