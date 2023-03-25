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
    public class LogController
    {
        public userTypes type = userTypes.none;
        public UserDTO userDTO;
        public LogController(IUserService userService)
        {
            this.userService = userService;
            string login = " ";
            bool incorrectInput = true;
            Viewer.ShowLoginForm();
            UserDTO user = new UserDTO();

            while (incorrectInput && login != dicCom[uC.Exit])
            {
                Viewer.ShowLogin();
                login = Console.ReadLine();
                Viewer.ShowPassword();
                string password = Console.ReadLine();
                
                try
                {
                    user = userService.GetUser(login, password);
                    incorrectInput = false;
                }
                catch (ValidationException ex)
                {
                    Viewer.Show(ex.Message);
                    incorrectInput = true;
                }
            }

            if (login != dicCom[uC.Exit])
            {
                userDTO = user;
                if (userTypes.admin.ToString() == user.Role)
                    type = userTypes.admin;
                if (userTypes.customer.ToString() == user.Role)
                    type = userTypes.customer;
            }

                     

        }

        public IUserService userService { get; set; }


    }
}
