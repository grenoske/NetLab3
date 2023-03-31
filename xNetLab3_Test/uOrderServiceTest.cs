using Moq;
using FluentAssertions;
using BLL.Infrastructure;
using BLL.Services;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Repositories;
using PL.Models;
using PL.View;
using BLL.DTO;
using DAL.Entities;
using System.Linq.Expressions;

namespace xNetLab3_Test
{
    public class uOrderServiceTest
    {
        private Mock<IUnitOfWork> mockUnitOfWork;
        private IuOrderService orderService;

        public uOrderServiceTest()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            orderService = new uOrderService(mockUnitOfWork.Object);
        }

        [Fact]
        public void MakeOrder_WithnullOrder_ErrorReturn()
        {
            // Arrange

            // Act
            Action action = new Action(() => orderService.MakeOrder(null));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("orderDto is null");
        }

        [Fact]
        public void MakeOrder_WithQuantityLessthanZero_ErrorReturn()
        {
            // Arrange
            uOrderDTO order = new uOrderDTO { Id = 1, Quantity = -1 };

            // Act
            Action action = new Action(() => orderService.MakeOrder(order));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Quantity cannot be less than Zero", "Quantity");
        }

        [Fact]
        public void MakeOrder_NoProduct_ProductErrorReturn()
        {
            // Arrange
            uOrderDTO order = new uOrderDTO { Id = 1 };

            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(Product));

            // Act
            Action action = new Action(() => orderService.MakeOrder(order));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Product not Found");
        }

        [Fact]
        public void MakeOrder_NoProductQuantity_ProductQuantityErrorReturn()
        {
            // Arrange
            uOrderDTO order = new uOrderDTO { Id = 1 };
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new Product());

            // Act
            Action action = new Action(() => orderService.MakeOrder(order));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("ProductQuantity not Found");
        }

        [Fact]
        public void MakeOrder_OrderWithIncorrectUserId_ErrorReturn()
        {
            // Arrange
            var userOrderRepository = new Mock<IuOrderRepository>();
            mockUnitOfWork.Setup(uow => uow.uOrders).Returns(userOrderRepository.Object);

            Product product = new Product { Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 1 };
            product.ProductQuantity = new ProductQuantity { Id = 1, ProductId = 1, Quantity = 100, ReservedQuantity = 0 };
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(product);

            mockUnitOfWork.Setup(u => u.Users.GetFirstOrDefault(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(User));

            uOrderDTO orderDTO = new uOrderDTO { Quantity = 100, Address = "2a" };

            // Act
            Action action = new Action(() => orderService.MakeOrder(orderDTO));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("User not found");
        }

        [Fact]
        public void MakeOrder_CorrectOrderWithEnoughQuantity()
        {
            // Arrange
            var userOrderRepository = new Mock<IuOrderRepository>();
            mockUnitOfWork.Setup(uow => uow.uOrders).Returns(userOrderRepository.Object);

            Product product = new Product { Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 1 };
            product.ProductQuantity = new ProductQuantity { Id = 1, ProductId = 1, Quantity = 100, ReservedQuantity = 0 };
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(product);

            User user = new User { Id = 1, Login = "TestUser", Password = "TestPass", Role = "customer" };
            mockUnitOfWork.Setup(u => u.Users.GetFirstOrDefault(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(user);

            uOrderDTO orderDTO = new uOrderDTO { Quantity=100, Address="2a" };

            // Act
            orderService.MakeOrder(orderDTO);

            // Assert
            userOrderRepository.Verify(repo => repo.Add(It.IsAny<uOrder>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void MakeOrder_CorrectOrderWithNOTEnoughQuantity()
        {
            // Arrange
            var userOrderRepository = new Mock<IuOrderRepository>();
            var whOrderRepository = new Mock<IwhOrderRepository>();
            mockUnitOfWork.Setup(uow => uow.uOrders).Returns(userOrderRepository.Object);
            mockUnitOfWork.Setup(uow => uow.whOrders).Returns(whOrderRepository.Object);

            Product product = new Product { Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 1 };
            product.ProductQuantity = new ProductQuantity { Id = 1, ProductId = 1, Quantity = 0, ReservedQuantity = 0 };
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(product);

            User user = new User { Id = 1, Login = "TestUser", Password = "TestPass", Role = "customer" };
            mockUnitOfWork.Setup(u => u.Users.GetFirstOrDefault(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(user);

            uOrderDTO orderDTO = new uOrderDTO { Quantity = 100, Address = "2a" };

            // Act
            orderService.MakeOrder(orderDTO);

            // Assert
            userOrderRepository.Verify(repo => repo.Add(It.IsAny<uOrder>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Save(), Times.Exactly(1));
        }

        [Fact]
        public void GetOrders_ListOrders_NotNullReturn()
        {
            // Arrange
            var mockOrderRepository = new Mock<IuOrderRepository>();

            mockOrderRepository.Setup(a => a.GetAll(null, default, default, null)).Returns(new List<uOrder>());
            mockUnitOfWork.Setup(a => a.uOrders).Returns(mockOrderRepository.Object);

            // Act
            var result = orderService.GetOrders();

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void GetProduct_ProductNotExist_Error()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(Product));

            int? id = 1;
            // Act
            Action action = new Action(() => orderService.GetProduct(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Product not Found");
        }

        [Fact]
        public void GetProduct_IdNull_Error()
        {
            // Arrange
            int? id = null;
            // Act
            Action action = new Action(() => orderService.GetProduct(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Id is null");
        }

        [Fact]
        public void GetProduct_ProductDTOReturn()
        {
            // Arrange
            int? id = 1;
            Product product = new Product { Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 1 };
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                p => p.Id == id,
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(product);
            
            // Act
            var productDTO = orderService.GetProduct(id);

            // Assert
            product.Should().BeEquivalentTo(productDTO);
        }

        [Fact]
        public void AddProduct_ProductDTONull_Error()
        {
            // Arrange
            ProductDTO productDTO = null;

            // Act
            Action action = new Action(() => orderService.AddProduct(productDTO));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("ProductDTO is null");
        }

        [Fact]
        public void AddProduct_ProductDTOIsAlreadyExists_Error()
        {
            // Arrange
            Product product = new Product { Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 1 };
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(product);

            ProductDTO productDTO = new ProductDTO() { Name = "TestProduct"};

            // Act
            Action action = new Action(() => orderService.AddProduct(productDTO));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Product is already exists");
        }

        [Fact]
        public void AddProduct_WithNewProduct_ProductQuantityAdded()
        {
            // Arrange
            var productQuantityRepository = new Mock<IProductQuantityRepository>();
            mockUnitOfWork.Setup(uow => uow.ProductQuantity).Returns(productQuantityRepository.Object);

            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(Product));
            ProductDTO productDTO = new ProductDTO { Name = "NewProduct", Company = "Company", Price = 1 };

            // Act
            orderService.AddProduct(productDTO);

            // Assert
            productQuantityRepository.Verify(repo => repo.Add(It.IsAny<ProductQuantity>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void RemoveProduct()
        {
            // Arrange
            Product product = new Product { Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 1 };
            var producRepository = new Mock<IProductRepository>();
            mockUnitOfWork.Setup(uow => uow.Products).Returns(producRepository.Object);

            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(product);

            // Act
            orderService.RemoveProduct(product.Id);

            // Assert
            producRepository.Verify(repo => repo.Remove(It.IsAny<Product>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void RemoveProduct_ProductNotExist_Error()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(Product));

            int id = 1;
            // Act
            Action action = new Action(() => orderService.RemoveProduct(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Product not Found");
        }

        [Fact]
        public void GetDetailProduct_IdNull_Error()
        {
            // Arrange
            int? id = null;
            // Act
            Action action = new Action(() => orderService.GetDetailProduct(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Id is null");
        }

        [Fact]
        public void GetDetailProduct_ProductNotExist_Error()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(Product));
            int? id = 1;

            // Act
            Action action = new Action(() => orderService.GetDetailProduct(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Product not Found");
        }

        [Fact]
        public void GetDetailProduct_ProductQuantityNotExist_Error()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new Product());
            int? id = 1;

            // Act
            Action action = new Action(() => orderService.GetDetailProduct(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Quantity not Found");
        }

        [Fact]
        public void GetDetailProduct_DetailProductDTOReturn()
        {
            // Arrange
            int? id = 1;
            Product product = new Product { Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 1 };
            product.ProductQuantity = new ProductQuantity { Id = 1, ProductId = 1, Quantity = 0, ReservedQuantity = 0, WarehouseId = 1 };
            DetailProductDTO detailProductDTOexpected = new DetailProductDTO { Id = product.Id, Name = product.Name, Company = product.Company, Price = product.Price, Quantity = product.ProductQuantity.Quantity, ReservedQuantity = product.ProductQuantity.ReservedQuantity };
            mockUnitOfWork.Setup(u => u.Products.GetFirstOrDefault(
                p => p.Id == id,
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(product);

            // Act
            var detailProductDTO = orderService.GetDetailProduct(id);

            // Assert
            detailProductDTO.Should().BeEquivalentTo(detailProductDTOexpected);
        }

        [Fact]
        public void GetDetailuOrder_IdNull_Error()
        {
            // Arrange
            int? id = null;
            // Act
            Action action = new Action(() => orderService.GetDetailuOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Id is null");
        }

        [Fact]
        public void GetDetailuOrder_OrderNotExist_Error()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(uOrder));
            int? id = 1;

            // Act
            Action action = new Action(() => orderService.GetDetailuOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Order not Found");
        }

        [Fact]
        public void GetDetailuOrder_ProductNotExist_Error()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new uOrder());
            int? id = 1;

            // Act
            Action action = new Action(() => orderService.GetDetailuOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Product not Found");
        }

        [Fact]
        public void GetDetailuOrder_UserNotExist_Error()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new uOrder { Product = new Product()});
            int? id = 1;

            // Act
            Action action = new Action(() => orderService.GetDetailuOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("User not Found");
        }

        [Fact]
        public void GetDetailuOrder_DetailuOrderDTO()
        {
            DateTime date = DateTime.Now;
            // Arrange
            uOrder uOrder = new uOrder { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Date = date, Sum = 1000, ProductId = 1, Product = new Product { Id = 1, Name = "TestProduct", Price = 100 }, User = new User { Id = 1, Login = "TestName"} };
            DetailuOrderDTO detailuOrderDTOexpected = new DetailuOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Date = date, Sum = 1000, ProductId = 1, ProductName = "TestProduct", ProductPrice = 100, UserId = 1, UserLogin = "TestName" };
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(uOrder);
            int? id = 1;

            // Act
            var result = orderService.GetDetailuOrder(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(detailuOrderDTOexpected);
        }

        [Fact]
        public void SearchProduct_NameNull()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },
                new Product{ Id = 2, Name = "TestProduct2", Company = "TestCompany2", Price = 100 },
                new Product{ Id = 3, Name = "ProductTest3", Company = "TestCompany3", Price = 100 },
            };

            var expectedProducts = new List<ProductDTO>
            {
                new ProductDTO{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },
                new ProductDTO{ Id = 2, Name = "TestProduct2", Company = "TestCompany2", Price = 100 },
                new ProductDTO{ Id = 3, Name = "ProductTest3", Company = "TestCompany3", Price = 100 },
            };

            mockUnitOfWork.Setup(u => u.Products.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(products);
            string Name = null;

            // Act
            var result = orderService.SearchProduct(Name);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public void SearchProduct_Name()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },
                new Product{ Id = 2, Name = "TestProduct2", Company = "TestCompany2", Price = 100 },
            };

            var expectedProducts = new List<ProductDTO>
            {
                new ProductDTO{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },
                new ProductDTO{ Id = 2, Name = "TestProduct2", Company = "TestCompany2", Price = 100 },
            };

            mockUnitOfWork.Setup(u => u.Products.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(products);
            string Name = "TestProduct";

            // Act
            var result = orderService.SearchProduct(Name);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public void GetUserOrders_Orders()
        {
            // Arrange
            DateTime datatime = DateTime.Now;
            var userOrders = new List<uOrder>
             {
                 new uOrder { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 }, User = new User { Id = 1, Login = "TestName", } },
                 new uOrder { Id = 2, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 }, User = new User { Id = 1, Login = "TestName", } },
             };

            var expecteduserOrders = new List<uOrderDTO>
             {
                 new uOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1},
                 new uOrderDTO { Id = 2, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1},
             };

            int? id = 1;
            mockUnitOfWork.Setup(u => u.uOrders.GetAll(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(userOrders);
            

            // Act
            var result = orderService.GetUserOrders(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expecteduserOrders);
        }

        [Fact]
        public void CancelOrder_NoOrder_ErrorReturn()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(uOrder));
            int id = 1;

            // Act
            Action action = new Action(() => orderService.CanceluOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Order not Found");
        }

        [Fact]
        public void CancelOrder_NoQuantity_ErrorReturn()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new uOrder { Id = 1});

            mockUnitOfWork.Setup(u => u.ProductQuantity.GetFirstOrDefault(
                It.IsAny<Expression<Func<ProductQuantity, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(ProductQuantity));

            int id = 1;

            // Act
            Action action = new Action(() => orderService.CanceluOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Quantity not Found");
        }

        [Fact]
        public void CancelOrder_NotCorrectOrederStatus_ErrorReturn()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new uOrder { Id = 1, Status="complete" });

            mockUnitOfWork.Setup(u => u.ProductQuantity.GetFirstOrDefault(
                It.IsAny<Expression<Func<ProductQuantity, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new ProductQuantity { Id = 1});


            int id = 1;
            // Act
            Action action = new Action(() => orderService.CanceluOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Order status isn't correct");
        }

        [Fact]
        public void CancelOrder_WarehouseOrderNotFound_ErrorReturn()
        {
            // Arrange
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new uOrder { Id = 1, Status = "wait" });

            mockUnitOfWork.Setup(u => u.ProductQuantity.GetFirstOrDefault(
                It.IsAny<Expression<Func<ProductQuantity, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new ProductQuantity { Id = 1 });

            int id = 1;
            // Act
            Action action = new Action(() => orderService.CanceluOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("WhOrder not Found");
        }

        [Fact]
        public void CancelOrder_AllCorrect()
        {
            // Arrange
            uOrder uOrder = new uOrder { Id = 1, Status = "wait", Quantity = 200, whOrder = new whOrder { Id = 1, Quantity = 100, Status = "wait" } };
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(uOrder);

            ProductQuantity productQuantity = new ProductQuantity { Id = 1, Quantity = 0, ReservedQuantity = 100 };
            mockUnitOfWork.Setup(u => u.ProductQuantity.GetFirstOrDefault(
                It.IsAny<Expression<Func<ProductQuantity, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(productQuantity);

            int id = 1;

            // Act
            orderService.CanceluOrder(id);

            // Assert
            uOrder.Status.Should().Be("cancel");
            uOrder.whOrder.Status.Should().Be("cancel");

            productQuantity.Quantity.Should().Be(100);

            mockUnitOfWork.Verify(uof => uof.ProductQuantity.Update(It.IsAny<ProductQuantity>()), Times.Once());
            mockUnitOfWork.Verify(uof => uof.uOrders.Update(It.IsAny<uOrder>()), Times.Once());
            mockUnitOfWork.Verify(uof => uof.Save(), Times.Once());
        }

        [Fact]
        public void SearchuOrder_AllCorrect()
        {
            // Arrange
            DateTime datatime = DateTime.Now;
            var userOrders = new List<uOrder>
             {
                 new uOrder { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 }, User = new User { Id = 1, Login = "TestName", } },
                 new uOrder { Id = 11, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 }, User = new User { Id = 1, Login = "TestName", } },
             };

            var expecteduserOrders = new List<uOrderDTO>
             {
                 new uOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1},
                 new uOrderDTO { Id = 11, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1},
             };

            int? id = 1;
            mockUnitOfWork.Setup(u => u.uOrders.GetAll(
                u => u.Id.ToString().Contains(id.ToString()),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(userOrders);


            // Act
            var result = orderService.SearchuOrder(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expecteduserOrders);
        }

        [Fact]
        public void SearchUserOrder_AllCorrect()
        {
            // Arrange
            DateTime datatime = DateTime.Now;
            var userOrders = new List<uOrder>
             {
                 new uOrder { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 }, User = new User { Id = 1, Login = "TestName", } },
                 new uOrder { Id = 11, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 }, User = new User { Id = 1, Login = "TestName", } },
             };

            var expecteduserOrders = new List<uOrderDTO>
             {
                 new uOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1},
                 new uOrderDTO { Id = 11, Address = "TestAddress", Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1},
             };

            int? id = 1;
            int? UserId = 1;
            mockUnitOfWork.Setup(u => u.uOrders.GetAll(
                u => u.UserId == UserId && u.Id.ToString().Contains(id.ToString()),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(userOrders);


            // Act
            var result = orderService.SearchUserOrder(id, UserId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expecteduserOrders);
        }

        [Fact]
        public void GetProducts_AllCorrect()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },
                new Product{ Id = 2, Name = "TestProduct2", Company = "TestCompany2", Price = 100 },
                new Product{ Id = 3, Name = "ProductTest3", Company = "TestCompany3", Price = 100 },
            };

            var expectedProducts = new List<ProductDTO>
            {
                new ProductDTO{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },
                new ProductDTO{ Id = 2, Name = "TestProduct2", Company = "TestCompany2", Price = 100 },
                new ProductDTO{ Id = 3, Name = "ProductTest3", Company = "TestCompany3", Price = 100 },
            };
            mockUnitOfWork.Setup(u => u.Products.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(products);

            // Act
            var result = orderService.GetProducts();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(expectedProducts);
        }



        [Fact]
        public void Dispose_Service()
        {
            // Arrange

            // Act
            orderService.Dispose();

            // Assert
            mockUnitOfWork.Verify(uow => uow.Dispose(), Times.Once);
        }
    }
}