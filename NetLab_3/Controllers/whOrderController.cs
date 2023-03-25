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
    public class whOrderController
    {
        public whOrderController(IuOrderService userOrdersService, IwhOrderService whOrdersService)
        {
            this.usOrderService = userOrdersService;
            this.whOrderService = whOrdersService;

        }

        public void DeliveryProductToWarehouse(int whOrderId)
        {
            Viewer.Show("--- DeliveryProductToWarehouse ---");
            Viewer.Show("-- Delivering.. --");
            try
            {
                whOrderService.DeliveryProductToWh(whOrderId);
            }
            catch(ValidationException ex)
            {
                Viewer.Show(ex.Message);
                Viewer.Show("-- -Delivering not complete --");
                return;
            }

        }

        public void DeliveryProductToUser(int usOrderId)
        {
            Viewer.Show("--- DeliveryProductToWarehouse ---");
            Viewer.Show("-- Delivering.. --");
            try
            {
                whOrderService.DeliveryProductToUser(usOrderId);
            }
            catch(ValidationException ex)
            {
                Viewer.Show(ex.Message);
                Viewer.Show("-- -Delivering not complete --");
                return;
            }

        }

        public void ListOfWhOrders()
        {
            Viewer.Show("--- List of Warehouse's Orders ---");
            var orders = whOrderService.GetOrders();
            foreach(var order in orders)
            {
                Viewer.Show(new whOrderViewModel(order).ToString());
            }
        }

        public void ListOfUsersOrders()
        {
            Viewer.Show("--- List of User's Orders ---");
            var orders = whOrderService.GetuOrders();
            foreach (var order in orders)
            {
                Viewer.Show(new uOrderViewModel(order).ToString());
            }
        }
        public IuOrderService usOrderService { get; set; }
        public IwhOrderService whOrderService { get; set; }


    }
}
