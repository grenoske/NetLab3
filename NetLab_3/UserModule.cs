using Ninject.Modules;
using BLL.Services;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Repositories;

namespace PL
{
    public class UserModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserService>().To<UserService>();
        }
    }
}
