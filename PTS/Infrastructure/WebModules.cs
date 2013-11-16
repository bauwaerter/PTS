using Ninject.Modules;
using Data;
using Core.Domains;
using Service;
using Service.Interfaces;
using Service.Services;

namespace PTS.Infrastructure
{
    public class WebModules : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserService>().To<UserService>();
            Bind<IStudentUserService>().To<StudentUserService>();
            //Bind<IRepository<StudentUser>>().To<Repository<StudentUser>>();
            //Bind(typeof(IBaseService<>)).To(typeof(BaseService<>));
        }
    }
}