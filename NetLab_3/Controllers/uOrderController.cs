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
    public class uOrderController
    {
        public uOrderController(IuOrderService orderService, UserDTO user)
        {

            this.orderService = orderService;
            userDTO = user;
        }

        public void MakeOrder()
        {
            uOrderDTO uOrder = new uOrderDTO();

            Viewer.ShowMakeOrder2();
            string orderInfo = Console.ReadLine();

            while (orderInfo != dicCom[uC.Exit])
            {
                orderInfo.Replace(" ", "");
                string[] orderInf = orderInfo.Split(';');
                if(orderInf.Length == 3)
                {
                    uOrder = new uOrderDTO()
                    {
                        ProductId = Convert.ToInt32(orderInf[0]),
                        Quantity = Convert.ToInt32(orderInf[1]),
                        Address = orderInf[2],
                        UserId = userDTO.Id
                    };
                    Viewer.AcceptInfo();
                    break;
                }
                Viewer.FormatErr();
            }

            try
            {
                this.orderService.MakeOrder(uOrder);
            }
            catch(ValidationException ex)
            {
                Viewer.Show(ex.Message);
                Viewer.ShowOrderErr();
                return;
            }

            Viewer.ShowOrderSuc();
        }

        public void ListOfOrders()
        {
            Viewer.Show("--- List of Yours Orders ---");
            var orders = orderService.GetOrders();
            foreach (var order in orders)
            {
                Viewer.Show(new uOrderViewModel(order).ToString());
            }
        }

        public void ListOfProducts()
        {
            Viewer.Show("--- List of Products ---");
            var products = orderService.GetProducts();
            foreach (var product in products)
            {
                Viewer.Show(new ProductViewModel(product).ToString());
            }
        }

        public void AddProduct()
        {

            ProductDTO product = new ProductDTO();

            Viewer.ShowCreateProduct();
            string productInfo = Console.ReadLine();

            while (productInfo != dicCom[uC.Exit])
            {
                productInfo.Replace(" ", "");
                string[] productInf = productInfo.Split(';');
                if (productInf.Length == 3)
                {
                    product = new ProductDTO()
                    {
                        Name = productInf[0],
                        Company = productInf[1],
                        Price = Convert.ToInt32(productInf[2]),
                    };
                    Viewer.AcceptInfo();
                    break;
                }
                Viewer.FormatErr();
            }
            try
            {
                this.orderService.AddProduct(product);
            }
            catch (ValidationException ex)
            {
                Viewer.Show(ex.Message);
                Viewer.ShowOrderErr();
                return;
            }

            Viewer.ShowProductSuc();

        }

        public IuOrderService orderService { get; set; }
        UserDTO userDTO { get; set; }


    }
}
