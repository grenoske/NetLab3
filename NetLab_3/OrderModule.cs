using Ninject.Modules;
using BLL.Services;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Repositories;
namespace PL
{
    public class OrderModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IuOrderService>().To<uOrderService>();
            Bind<IwhOrderService>().To<whOrderService>();
        }
    }
}
