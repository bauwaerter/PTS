using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;

namespace Blizzard.Infrastructure {

    public class NinjectScope : IDependencyScope{

        private IResolutionRoot _resolution;

        public NinjectScope(IResolutionRoot root){
            _resolution = root;
        }

        public object GetService(Type serviceType){
            if (_resolution == null)
                throw new ObjectDisposedException("this", "This scope has been disposed");

            return _resolution.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType){
            if (_resolution == null)
                throw new ObjectDisposedException("this", "This scope has been disposed");

            return _resolution.GetAll(serviceType);
        }

        public void Dispose(){

            var disposable = _resolution as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            _resolution = null;
        }
    }
}
