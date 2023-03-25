using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using DAL.Entities;
using DAL.Interfaces;
using BLL.Infrastructure;
using BLL.Interfaces;
using AutoMapper;

namespace BLL.Services
{
    public class uOrderService : IuOrderService
    {
        enum orderStats { wait, ready, complete, cancel }
        Dictionary<orderStats, string> dicStats = new Dictionary<orderStats, string>
        {
            { orderStats.wait, "wait" },            // wait for product to warehouse
            { orderStats.ready, "ready" },          // ready, product is in warehouse, need to deliver
            { orderStats.complete, "complete" },    // is completed order
            { orderStats.cancel, "cancel"},         // order is canceled

        };

        IUnitOfWork Database { get; set; }
        public uOrderService(IUnitOfWork unitOfWork)
        {
            Database = unitOfWork;
        }
        public void MakeOrder(uOrderDTO orderDto) 
        {
            if (orderDto == null)
                throw new ValidationException("orderDto is null", "");
            if (orderDto.Quantity < 0)
                throw new ValidationException("Quantity cannot be less than Zero", "Quantity");

            // Getting product
            Product product;
            product = Database.Products.GetFirstOrDefault(u => u.Id == orderDto.ProductId, "ProductQuantity");
            if (product == null)
                throw new ValidationException("Product not Found", "");
            if (product.ProductQuantity == null)
                throw new ValidationException("ProductQuantity not Found", "");

            var date = DateTime.Now; 

            // forming userOrder
            uOrder uOrder = new uOrder()
            {
                Sum = orderDto.Quantity * product.Price,
                Quantity = orderDto.Quantity,
                Address = orderDto.Address,
                
                Date = date,
            };

            if (product.ProductQuantity.Quantity < orderDto.Quantity) // if quantity not enough
            {
                // forming order for warehouse
                whOrder whNewOrder = new whOrder()
                {
                    Quantity = orderDto.Quantity - product.ProductQuantity.Quantity, // sub the quantity that already in warehouse
                    Status = dicStats[orderStats.wait],
                    Date = date,
                };

                // no product is remain after reservation for order
                product.ProductQuantity.Quantity = 0;
                product.ProductQuantity.ReservedQuantity = product.ProductQuantity.Quantity;

                whNewOrder.Product = product;
                whNewOrder.Sum = whNewOrder.Quantity * product.Price;
                //Database.whOrders.Add(whNewOrder);
                uOrder.whOrder = whNewOrder;

                // wait till wh complete
                uOrder.Status = dicStats[orderStats.wait];
            }
            else
            {
                // reserve quanity for order
                product.ProductQuantity.Quantity -= orderDto.Quantity;
                product.ProductQuantity.ReservedQuantity = orderDto.Quantity;

                // ready for delivery
                uOrder.Status = dicStats[orderStats.ready];
            }

            User user = Database.Users.GetFirstOrDefault(u => u.Id == orderDto.UserId);
            if (user == null)
                throw new ValidationException("User not found", "");


            uOrder.Product = product;
            uOrder.User = user;

            Database.uOrders.Add(uOrder);
            // update database
            Database.Save();
        }

        public void CanceluOrder(int id)
        {
            var order = Database.uOrders.GetFirstOrDefault(u => u.Id == id, "whOrder");
            if (order == null)
                throw new ValidationException("Order not Found", "");
            var quantity = Database.ProductQuantity.GetFirstOrDefault(pq => pq.ProductId == order.ProductId);
            if (quantity == null)
                throw new ValidationException("Quantity not Found", "");
            if(order.Status == dicStats[orderStats.complete] || order.Status == dicStats[orderStats.cancel])
                throw new ValidationException("Order status isn't correct", "");

            if (order.Status == dicStats[orderStats.wait]) // it is mean that need to cancel whOrder first
            {
                if (order.whOrder == null)
                    throw new ValidationException("WhOrder not Found", "");

                order.whOrder.Status = dicStats[orderStats.cancel];

                // need to calculate amount of product that was reserved and be physically in wh
                int PartQuantityReserved = order.Quantity - order.whOrder.Quantity;
                order.Quantity = PartQuantityReserved;
            }
            order.Status = dicStats[orderStats.cancel];

            // back quantity from reserv to main stock
            quantity.ReservedQuantity -= order.Quantity;
            quantity.Quantity += order.Quantity;

            Database.ProductQuantity.Update(quantity);
            Database.uOrders.Update(order);
            Database.Save();
        }

        public ProductDTO GetProduct(int? id)
        {
            if (id == null)
                throw new ValidationException("Id is null", "");
            var product = Database.Products.GetFirstOrDefault(u => u.Id == id);
            if (product == null)
                throw new ValidationException("Product not Found", "");

            return new ProductDTO { Company = product.Company, Id = product.Id, Name = product.Name, Price = product.Price };
        }

        public void AddProduct(ProductDTO productDTO)
        {
            if (productDTO == null)
                throw new ValidationException("ProductDTO is null", "");
            if (Database.Products.GetFirstOrDefault(u => u.Name == productDTO.Name) != null)
            {
                throw new ValidationException("Product is already exists", "");
            }

            Product product = new Product()
            {
                Name = productDTO.Name,
                Company = productDTO.Company,
                Price = productDTO.Price,
            };
            
            ProductQuantity productQuantity = new ProductQuantity()
            {
                Quantity = 0,
                WarehouseId = 1
            };

            productQuantity.Product = product;
            Database.ProductQuantity.Add(productQuantity);
            Database.Save();

        }
        public void RemoveProduct(int id)
        {
            var product = Database.Products.GetFirstOrDefault(u => u.Id == id, "ProductQuantity");
            if (product == null)
                throw new ValidationException("Product not Found", "");

            Database.Products.Remove(product);
            Database.Save();

        }

        public DetailProductDTO GetDetailProduct(int? id)
        {
            if (id == null)
                throw new ValidationException("Id is null", "");
            var product = Database.Products.GetFirstOrDefault(u => u.Id == id, "ProductQuantity", false);
            if (product == null)
                throw new ValidationException("Product not Found", "");
            if (product.ProductQuantity == null)
                throw new ValidationException("Quantity not Found", "");

            return new DetailProductDTO { Company = product.Company, Id = product.Id, Name = product.Name, Price = product.Price, Quantity=product.ProductQuantity.Quantity, ReservedQuantity=product.ProductQuantity.ReservedQuantity };
        }
        public DetailuOrderDTO GetDetailuOrder(int? id)
        {
            if (id == null)
                throw new ValidationException("Id is null", "");
            var uOrder = Database.uOrders.GetFirstOrDefault(o => o.Id == id, "Product,User", false);
            if(uOrder == null)
                throw new ValidationException("Order not Found", "");
            if (uOrder.Product == null)
                throw new ValidationException("Product not Found", "");
            if (uOrder.User == null)
                throw new ValidationException("User not Found", "");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<uOrder, DetailuOrderDTO>()).CreateMapper();
            var detailuOrderDTO = mapper.Map<uOrder,DetailuOrderDTO>(uOrder);
            detailuOrderDTO.ProductName = uOrder.Product.Name;
            detailuOrderDTO.ProductCompany = uOrder.Product.Company;
            detailuOrderDTO.ProductPrice = uOrder.Product.Price;
            detailuOrderDTO.UserLogin = uOrder.User.Login;
            detailuOrderDTO.UserId = uOrder.User.Id;

            return detailuOrderDTO;

        }

        public IEnumerable<ProductDTO> SearchProduct(string Name)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Product>, List<ProductDTO>>(Database.Products.GetAll(p => p.Name.Contains(Name)));
        }

        public IEnumerable<uOrderDTO> SearchuOrder(int? id)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<uOrder, uOrderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<uOrder>, List<uOrderDTO>>(Database.uOrders.GetAll(u => u.Id.ToString().Contains(id.ToString())));
        }

        public IEnumerable<uOrderDTO> SearchUserOrder(int? id, int? UserId)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<uOrder, uOrderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<uOrder>, List<uOrderDTO>>(Database.uOrders.GetAll(u => u.UserId == UserId && u.Id.ToString().Contains(id.ToString())));
        }

        public IEnumerable<uOrderDTO> GetOrders()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<uOrder, uOrderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<uOrder>, List<uOrderDTO>>(Database.uOrders.GetAll());
        }

        public IEnumerable<uOrderDTO> GetUserOrders(int? Userid, int page = 1, int amount = 5)
        {
            var UserOrders = Database.uOrders.GetAll(u => u.UserId == Userid, page, amount);
         
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<uOrder, uOrderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<uOrder>, List<uOrderDTO>>(UserOrders);
        }

        public IEnumerable<ProductDTO> GetProducts(int page = 1, int amount = 5)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductDTO>()).CreateMapper();
            var productsDTO = mapper.Map<IEnumerable<Product>, List<ProductDTO>>(Database.Products.GetAll(null, page, amount));
            return productsDTO;
        }


        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
