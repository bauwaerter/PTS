using System;
using Ninject.Modules;
using Service.Interfaces;

namespace Blizzard.Infrastructure {

    public class WebDependancyModules : NinjectModule {

        public override void Load(){

            Bind<IUserService>().To<UserService>();
        }
    }
}
