using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using DAL.Entities;
// using BLL.BusinessModels
using DAL.Interfaces;
using BLL.Infrastructure;
using BLL.Interfaces;
using AutoMapper;

namespace BLL.Services
{
    public class whOrderService : IwhOrderService
    {
        enum orderStats { wait, ready, complete, cancel}
        Dictionary<orderStats, string> dicStats = new Dictionary<orderStats, string>
        {
            { orderStats.wait, "wait" },            // wait for product to warehouse
            { orderStats.ready, "ready" },          // ready, product is in warehouse, need to deliver
            { orderStats.complete, "complete" },    // is completed order
            { orderStats.cancel, "cancel" }         // order is canceled

        };

        IUnitOfWork Database { get; set; }
        public whOrderService(IUnitOfWork unitOfWork)
        {
            Database = unitOfWork;
        }
        public void DeliveryProductToWh(int?  wOrderDtoID = null) 
        {
            // Getting WareHouseOrder
            whOrder wOrder;
            if (wOrderDtoID == null)  // id = null - process next delivery to wh
            {
                wOrder = Database.whOrders.GetFirstOrDefault(u => u.Status == dicStats[orderStats.wait], "Product,uOrder");
            }
            else   // concrete id delivery
            {
                wOrder = Database.whOrders.GetFirstOrDefault(u => u.Id == wOrderDtoID && u.Status == dicStats[orderStats.wait], "Product,uOrder");
            }

            if (wOrder == null)
                throw new ValidationException("Order not Found", "");

            if (wOrder.Status == dicStats[orderStats.complete])
                throw new ValidationException("Order is already completed", "");

            if (wOrder.Product == null) // if product not found
            {
                throw new ValidationException("Product not Found", "");
            }

            ProductQuantity quantity = Database.ProductQuantity.GetFirstOrDefault(u => u.ProductId == wOrder.ProductId);
            if (quantity == null)
            {
                throw new ValidationException("Quantity not Found", "");
            }

            // Delivering product to warehouse
            quantity.ReservedQuantity += wOrder.Quantity;
            Database.ProductQuantity.Update(quantity);

            // Update orders status
            wOrder.Status = dicStats[orderStats.complete];
            wOrder.uOrder.Status = dicStats[orderStats.ready];
            Database.whOrders.Update(wOrder);

            Database.Save();
        }

        public void DeliveryProductToUser(int? uOrderDtoId = null)
        {
            uOrder userOrder;
            if (uOrderDtoId == null) // if nextDelivery to user
            {
                userOrder = Database.uOrders.GetFirstOrDefault(u => u.Status == dicStats[orderStats.ready]);
            }
            else   // concrete uOrder id
            {
                userOrder = Database.uOrders.GetFirstOrDefault(u => u.Id == uOrderDtoId && u.Status == dicStats[orderStats.ready]);
            }
            if (userOrder == null)
                throw new ValidationException("UserOrder not Found", "");
            if (userOrder.Status != dicStats[orderStats.ready])
                throw new ValidationException("Order status isn't correct", "");

            ProductQuantity productQuantity = Database.ProductQuantity.GetFirstOrDefault(u => u.ProductId == userOrder.ProductId);
            if (productQuantity == null)
                throw new ValidationException("Quantity not Found", "");
            // delivering to customer
            productQuantity.ReservedQuantity -= userOrder.Quantity;
            userOrder.Status = dicStats[orderStats.complete];
            Database.ProductQuantity.Update(productQuantity);
            Database.uOrders.Update(userOrder);
            Database.Save();
        }
        public DetailwhOrderDTO GetDetailwhOrder(int? id)
        {
            if (id == null)
                throw new ValidationException("Id is null", "");
            var whOrder = Database.whOrders.GetFirstOrDefault(o => o.Id == id, "Product", false);
            if (whOrder == null)
                throw new ValidationException("Order not Found", "");
            if (whOrder.Product == null)
                throw new ValidationException("Product not Found", "");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<whOrder, DetailwhOrderDTO>()).CreateMapper();
            var detailwhOrderDTO = mapper.Map<whOrder, DetailwhOrderDTO>(whOrder);
            detailwhOrderDTO.ProductName = whOrder.Product.Name;
            detailwhOrderDTO.ProductCompany = whOrder.Product.Company;
            detailwhOrderDTO.ProductPrice = whOrder.Product.Price;

            return detailwhOrderDTO;
        }

        public IEnumerable<whOrderDTO> SearchwhOrder(int? id)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<whOrder, whOrderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<whOrder>, List<whOrderDTO>>(Database.whOrders.GetAll(u => u.Id.ToString().Contains(id.ToString())));
        }
        public IEnumerable<whOrderDTO> GetOrders(string? filter = null, int page = 1, int amount = 5)
        {
            if (filter == "purchase")
            {
                var mapper1 = new MapperConfiguration(cfg => cfg.CreateMap<whOrder, whOrderDTO>()).CreateMapper();
                var whWOrders = mapper1.Map<IEnumerable<whOrder>, List<whOrderDTO>>(Database.whOrders.GetAll(u => u.Status == dicStats[orderStats.wait], page, amount));
                return whWOrders;
            }
            else
            {
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<whOrder, whOrderDTO>()).CreateMapper();
                var whOrders = mapper.Map<IEnumerable<whOrder>, List<whOrderDTO>>(Database.whOrders.GetAll(null, page, amount));
                return whOrders;
            }

        }

        public IEnumerable<uOrderDTO> GetuOrders(string? filter = null, int page = 1, int amount = 5)
        {
            if (filter == "delivery")
            {
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<uOrder, uOrderDTO>()).CreateMapper();
                var uDOrders = mapper.Map<IEnumerable<uOrder>, List<uOrderDTO>>(Database.uOrders.GetAll(u => u.Status == dicStats[orderStats.ready], page, amount));
                return uDOrders;
            }
            else
            {
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<uOrder, uOrderDTO>()).CreateMapper();
                var uDOrders = mapper.Map<IEnumerable<uOrder>, List<uOrderDTO>>(Database.uOrders.GetAll(null, page, amount));
                return uDOrders;
            }
            
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
