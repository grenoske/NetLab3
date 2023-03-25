using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.Controllers;
using Microsoft.Extensions.Configuration;
using Ninject;
using Ninject.Modules;
using BLL.Infrastructure;
using BLL.Services;
using BLL.Interfaces;
using DAL.Interfaces;
using PL.Models;
using PL.View;
using BLL.DTO;


namespace PL
{
    class Program
    {
        
        static void Main(string[] args)
        {
            ForConfig config = new ForConfig();
            ContainerLoadGet container = new ContainerLoadGet(config.ConnectionString);


            HomeController controller = new HomeController(container.userOrderService, container.whOrderService, container.userService);



            Console.WriteLine("---- Program ends its execution -----");
        }

    }
}
