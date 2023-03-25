using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;
using BLL.Infrastructure;
using BLL.Services;
using BLL.Interfaces;
using DAL.Interfaces;
using PL.Models;
using PL.View;
using BLL.DTO;
using Microsoft.Extensions.Configuration;
using static PL.AE;

namespace PL.Controllers
{
    public class RegController
    {
        public RegController(IUserService userService)
        {
            this.userService = userService;
            string login = " ";
            bool incorrectInput = true;
            Viewer.ShowRegForm();

            while (incorrectInput && login != dicCom[uC.Exit])
            {
                Viewer.ShowLogin();
                login = Console.ReadLine();
                Viewer.ShowPassword();
                string password = Console.ReadLine();
                UserDTO user = new UserDTO();
                try
                {
                    user = userService.GetUser(login, password);
                    incorrectInput = true;
                }
                catch (ValidationException ex)
                {
                    userService.RegUser(new UserDTO() { Date = DateTime.Now, Login = login, Password = password, Role = "customer" });
                    Viewer.Show("successful registraion");
                    incorrectInput = false;
                }
            }

        }

        public IUserService userService { get; set; }


    }
}
