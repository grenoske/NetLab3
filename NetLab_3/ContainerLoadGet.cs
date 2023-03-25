using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Interfaces;
using Ninject;
using Ninject.Modules;
using BLL.Infrastructure;

namespace PL
{
    public class ContainerLoadGet
    {
        public ContainerLoadGet(string connectionString)
        {
            NinjectModule serviceModule = new ServiceModule(connectionString);
            IKernel kernel = new StandardKernel(serviceModule);
            kernel.Load<OrderModule>();
            kernel.Load<UserModule>();
            IUnitOfWork uiw = kernel.Get<IUnitOfWork>();
            IuOrderService userOrderService = kernel.Get<IuOrderService>();
            IwhOrderService whOrderService = kernel.Get<IwhOrderService>();
            IUserService userService = kernel.Get<IUserService>();

            this.userOrderService = userOrderService;
            this.whOrderService = whOrderService;
            this.userService = userService;
        }

        public IuOrderService userOrderService { get; set; }
        public IUserService userService { get; set; }
        public IwhOrderService whOrderService { get; set; }
    }
}
