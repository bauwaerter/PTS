using Ninject.Modules;
using Data;
using Core.Domains;

namespace Service
{
    public class ServiceModules : NinjectModule
    {
        public override void Load()
        {
            //Bind<IDbContext>().To<AppContext>();
            Bind<IRepository<User>>().To<Repository<User>>();
            //Bind<IRepository<StudentUser>>().To<Repository<StudentUser>>();
        }
    }
}
