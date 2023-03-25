using System;
using BLL.Interfaces;
using PL.View;
using BLL.DTO;
using static PL.AE;

namespace PL.Controllers
{
    public class HomeController
    {
        
        public HomeController(IuOrderService ous, IwhOrderService ows, IUserService us)
        {
            userOrderService = ous;
            warehOrderService = ows;
            userService = us;
            string command = " ";
            UserDTO user = new UserDTO();
            userTypes type = userTypes.none;

            while (command != dicCom[uC.Exit])
            {
                // admin
                while (command != dicCom[uC.Exit] && type == userTypes.admin)
                {
                    Viewer.ShowAdminCommandList();
                    command = Console.ReadLine();
                    whOrderController whOrderController1 = new whOrderController(userOrderService, warehOrderService);
                    if (command == dicCom[uC.ListOfWhOrders])
                    {
                        whOrderController1.ListOfWhOrders();
                    }
                    else if (command == dicCom[uC.ListOfUsersOrders])
                    {
                        whOrderController1.ListOfUsersOrders();
                    }
                    else if (command == dicCom[uC.DeliveryProductToWarehouse])
                    {
                        Viewer.ShowInputWareHouseOrderId();
                        int intTemp = Convert.ToInt32(Console.ReadLine());

                        whOrderController1.DeliveryProductToWarehouse(intTemp);
                    }
                    else if (command == dicCom[uC.DeliveryProductToUser])
                    {
                        Viewer.ShowInputUserOrderId();
                        int intTemp = Convert.ToInt32(Console.ReadLine());

                        whOrderController1.DeliveryProductToUser(intTemp);
                    }
                }

                // customer
                while (command != dicCom[uC.Exit] && type == userTypes.customer)
                {
                    Viewer.ShowWelcomeCustomer();
                    Viewer.ShowCostumerCommandList();
                    command = Console.ReadLine();
                    uOrderController uOrderController1 = new uOrderController(userOrderService, user);
                    if (command == dicCom[uC.MakeOrder])
                    {
                        uOrderController1.MakeOrder();
                    }
                    else if (command == dicCom[uC.ProductList])
                    {
                        uOrderController1.ListOfProducts();
                    }
                    else if (command == dicCom[uC.MyOrders])
                    {
                        uOrderController1.ListOfOrders();
                    }
                    else if (command == dicCom[uC.AddProduct])
                    {
                        uOrderController1.AddProduct();
                    }
                }
                type = userTypes.none;
                command = " ";

                // not authorized user
                while (command != dicCom[uC.Exit] && type == userTypes.none)
                {
                    Viewer.ShowWelcomeMessage();
                    Viewer.ShowHomeCommandList();
                    command = Console.ReadLine();
                    if (command == dicCom[uC.LogIn])
                    {
                        LogController log = new LogController(userService);
                        type = log.type;
                        user = log.userDTO;
                    }
                    else if (command == dicCom[uC.Reg])
                    {
                        RegController reg = new RegController(userService);
                    }
                }

            }

        }

        public IuOrderService userOrderService { get; set; }
        public IwhOrderService warehOrderService { get; set; }
        public IUserService userService { get; set; }

    }
}
