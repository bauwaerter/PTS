using System;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;

namespace PTS.Infrastructure
{
    /// <summary>
    /// Provides a Ninject implementation of IDependencyScope 
    /// which resolves services using the Ninject container.
    /// </summary>
    public class NinjectScope : IDependencyScope
    {
        IResolutionRoot resolver;

        public NinjectScope(IResolutionRoot resolver)
        {
            this.resolver = resolver;
        }

        public object GetService(Type serviceType)
        {
            if (resolver == null)
                throw new ObjectDisposedException("this", "This scope has been disposed");

            return resolver.TryGet(serviceType);
        }

        public System.Collections.Generic.IEnumerable<object> GetServices(Type serviceType)
        {
            if (resolver == null)
                throw new ObjectDisposedException("this", "This scope has been disposed");

            return resolver.GetAll(serviceType);
        }

        public void Dispose()
        {
            var disposable = resolver as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            resolver = null;
        }
    }
}