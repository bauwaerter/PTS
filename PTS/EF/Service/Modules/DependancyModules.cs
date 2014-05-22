using Core.Domains;
using Map.Repo;
using Ninject.Modules;
using Service.Interfaces;

namespace Service {
    public class DependancyModules : NinjectModule{

        public override void Load(){

            Bind<IDbContext>().To<AppContext>();
            Bind<IRepository<User>>().To<Repository<User>>();
        }
    }
}
