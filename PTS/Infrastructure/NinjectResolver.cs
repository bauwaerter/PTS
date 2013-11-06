using System.Web.Http.Dependencies;
using Ninject;

namespace PTS.Infrastructure
{
    /// <summary>
    /// This class is the resolver, but it is also the global scope
    /// so we derive from NinjectScope.
    /// </summary>
    public class NinjectResolver : NinjectScope, IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectResolver(IKernel kernel)
            : base(kernel)
        {
            _kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectScope(_kernel.BeginBlock());
        }

    } // class
} // namespace