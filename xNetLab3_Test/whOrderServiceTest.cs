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
    public class whOrderServiceTest
    {
        [Fact]
        public void DeliveryProductToWh_WithNotCorrectWhOrder_ErrorReturn()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.whOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(whOrder));

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            int wOrderDtoID = 1;

            // Act
            Action action = new Action(() => orderService.DeliveryProductToWh(wOrderDtoID));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Order not Found");
        }

        [Fact]
        public void DeliveryProductToWh_WithNoWhOrder_ErrorReturn()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.whOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(whOrder));

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);

            // Act
            Action action = new Action(() => orderService.DeliveryProductToWh());

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Order not Found");
        }

        [Fact]
        public void DeliveryProductToWh_CompleteStatus_ErrorReturn()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.whOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new whOrder { Id = 1, Status = "complete"});

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            int wOrderDtoID = 1;

            // Act
            Action action = new Action(() => orderService.DeliveryProductToWh(wOrderDtoID));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Order is already completed");
        }

        [Fact]
        public void DeliveryProductToWh_NoProduct_ErrorReturn()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.whOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new whOrder { Id = 1, Status = "ready" });

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            int wOrderDtoID = 1;

            // Act
            Action action = new Action(() => orderService.DeliveryProductToWh(wOrderDtoID));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Product not Found");
        }

        [Fact]
        public void DeliveryProductToWh_NoProductQuantity_ErrorReturn()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.whOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new whOrder { Id = 1, Status = "ready", Product = new Product()});

            mockUnitOfWork.Setup(u => u.ProductQuantity.GetFirstOrDefault(
                It.IsAny<Expression<Func<ProductQuantity, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(ProductQuantity));

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            int wOrderDtoID = 1;

            // Act
            Action action = new Action(() => orderService.DeliveryProductToWh(wOrderDtoID));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Quantity not Found");
        }

        [Fact]
        public void DeliveryProductToWh_AllCorrect()
        {
            DateTime date = DateTime.Now;
            // Arrange
            ProductQuantity quantity = new ProductQuantity { Id = 1, ProductId = 1, Quantity = 100, ReservedQuantity = 100 };
            var userOrder = new uOrder { Id = 1, Address = "TestAddress", Quantity = 100, Status = "wait", Date = date, Sum = 1000, ProductId = 1, Product = new Product { Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 }, User = new User { Id = 1, Login = "TestName", }, whOrderId = 1 };
            whOrder whorder = new whOrder { Id = 1, Sum = 100, Status = "wait", Quantity = 100, Date = date, ProductId = 1, Product = new Product { Id = 1, Company = "TestCompany", Name = "TestProduct", Price = 100 }, uOrder = userOrder, uOrderId = 1};
            int expectedQuantity = quantity.ReservedQuantity + whorder.Quantity;

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var quantityRepository = new Mock<IProductQuantityRepository>();
            var whOrderRepository = new Mock<IwhOrderRepository>();
            var uOrderRepository = new Mock<IuOrderRepository>();
            mockUnitOfWork.Setup(uow => uow.ProductQuantity).Returns(quantityRepository.Object);
            mockUnitOfWork.Setup(uow => uow.whOrders).Returns(whOrderRepository.Object);

            mockUnitOfWork.Setup(u => u.whOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(whorder);

            mockUnitOfWork.Setup(u => u.ProductQuantity.GetFirstOrDefault(
                It.IsAny<Expression<Func<ProductQuantity, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(quantity);

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            int wOrderDtoID = 1;

            // Act
            orderService.DeliveryProductToWh(wOrderDtoID);

            // Assert
            quantity.ReservedQuantity.Should().Be(expectedQuantity);
            quantityRepository.Verify(repo => repo.Update(It.IsAny<ProductQuantity>()), Times.Once);

            whorder.Status.Should().Be("complete");
            whOrderRepository.Verify(repo => repo.Update(It.IsAny<whOrder>()), Times.Once);


            mockUnitOfWork.Verify(uof => uof.Save(), Times.Once);
        }


        [Fact]
        public void DeliveryProductToUser_WithNotCorrectWhOrder_ErrorReturn()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(uOrder));

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            int uOrderDtoId = 1;

            // Act
            Action action = new Action(() => orderService.DeliveryProductToUser(uOrderDtoId));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("UserOrder not Found");
        }

        [Fact]
        public void DeliveryProductToUser_WithNoWhOrders_ErrorReturn()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(uOrder));

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);;

            // Act
            Action action = new Action(() => orderService.DeliveryProductToUser());

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("UserOrder not Found");
        }

        [Fact]
        public void DeliveryProductToUser_WithNotCorrectStatus_ErrorReturn()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new uOrder { Id = 1, Status = "complete" });

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            int uOrderDtoId = 1;

            // Act
            Action action = new Action(() => orderService.DeliveryProductToUser(uOrderDtoId));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Order status isn't correct");
        }

        [Fact]
        public void DeliveryProductToUser_WithQuantityNull_ErrorReturn()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new uOrder { Id = 1, Status = "ready" });

            mockUnitOfWork.Setup(u => u.ProductQuantity.GetFirstOrDefault(
                It.IsAny<Expression<Func<ProductQuantity, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(ProductQuantity));

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            int uOrderDtoId = 1;

            // Act
            Action action = new Action(() => orderService.DeliveryProductToUser(uOrderDtoId));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Quantity not Found");
        }

        [Fact]
        public void DeliveryProductToUser_Correct()
        {
            // Arrange
            DateTime date = DateTime.Now;
            ProductQuantity quantity = new ProductQuantity { Id = 1, ProductId = 1, Quantity = 100, ReservedQuantity = 100 };
            uOrder uorder = new uOrder { Id = 1, Address = "TestAddress", Quantity = 100, Status = "ready", Date = date, Sum = 1000, ProductId = 1, Product = new Product { Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 }, User = new User { Id = 1, Login = "TestName" } };
            
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var quantityRepository = new Mock<IProductQuantityRepository>();
            var uOrderRepository = new Mock<IuOrderRepository>();
            mockUnitOfWork.Setup(uow => uow.ProductQuantity).Returns(quantityRepository.Object);
            mockUnitOfWork.Setup(uow => uow.uOrders).Returns(uOrderRepository.Object);

            mockUnitOfWork.Setup(u => u.uOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(uorder);

            mockUnitOfWork.Setup(u => u.ProductQuantity.GetFirstOrDefault(
                It.IsAny<Expression<Func<ProductQuantity, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(quantity);

            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            int uOrderDtoId = 1;

            // Act
            orderService.DeliveryProductToUser(uOrderDtoId);

            // Assert
            quantity.ReservedQuantity.Should().Be(0);
            quantityRepository.Verify(repo => repo.Update(It.IsAny<ProductQuantity>()), Times.Once);

            uorder.Status.Should().Be("complete");
            uOrderRepository.Verify(repo => repo.Update(It.IsAny<uOrder>()), Times.Once);

            mockUnitOfWork.Verify(uof => uof.Save(), Times.Once);
        }

        [Fact]
        public void GetDetailwhOrder_IdNull_Error()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            int? id = null;
            // Act
            Action action = new Action(() => orderService.GetDetailwhOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Id is null");
        }

        [Fact]
        public void GetDetailuOrder_OrderNotExist_Error()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            mockUnitOfWork.Setup(u => u.whOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(default(whOrder));
            int? id = 1;

            // Act
            Action action = new Action(() => orderService.GetDetailwhOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Order not Found");
        }

        [Fact]
        public void GetDetailuOrder_ProductNotExist_Error()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            mockUnitOfWork.Setup(u => u.whOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(new whOrder());
            int? id = 1;

            // Act
            Action action = new Action(() => orderService.GetDetailwhOrder(id));

            // Assert
            action.Should().Throw<ValidationException>()
                .WithMessage("Product not Found");
        }

        [Fact]
        public void GetDetailuOrder_DetailuOrderDTO()
        {
            // Arrange
            whOrder whOrder = new whOrder { Id = 1, Quantity = 100, Status = "test", Date = DateTime.Now, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },};
            DetailwhOrderDTO expectedDetailwhOrderDTO = new DetailwhOrderDTO { Id = whOrder.Id, Quantity = whOrder.Quantity, Status = whOrder.Status, Date = whOrder.Date, Sum = whOrder.Sum, ProductId = whOrder.ProductId, ProductName = whOrder.Product.Name, ProductCompany = whOrder.Product.Company, ProductPrice = whOrder.Product.Price };
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            mockUnitOfWork.Setup(u => u.whOrders.GetFirstOrDefault(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Returns(whOrder);
            int? id = 1;

            // Act
            var result = orderService.GetDetailwhOrder(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDetailwhOrderDTO);
        }

        [Fact]
        public void GetOrders_PurchaseOrders()
        {
            // Arrange
            DateTime datatime = DateTime.Now;
            var whOrders = new List<whOrder>
             {
                 new whOrder { Id = 1,  Quantity = 100, Status = "wait", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 } },
                 new whOrder { Id = 2,  Quantity = 100, Status = "wait", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 } },
             };

            var expecteduserOrders = new List<whOrderDTO>
             {
                 new whOrderDTO { Id = 1, Quantity = 100, Status = "wait", Date = datatime, Sum = 1000, ProductId = 1},
                 new whOrderDTO { Id = 2, Quantity = 100, Status = "wait", Date = datatime, Sum = 1000, ProductId = 1},
             };

            int? id = 1;
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            mockUnitOfWork.Setup(u => u.whOrders.GetAll(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(whOrders);


            // Act
            var result = orderService.GetOrders("purchase");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expecteduserOrders);
        }

        [Fact]
        public void GetOrders_Orders()
        {
            // Arrange
            DateTime datatime = DateTime.Now;
            var whOrders = new List<whOrder>
             {
                 new whOrder { Id = 1,  Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 } },
                 new whOrder { Id = 2,  Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 } },
             };

            var expecteduserOrders = new List<whOrderDTO>
             {
                 new whOrderDTO { Id = 1, Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1},
                 new whOrderDTO { Id = 2, Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1},
             };

            int? id = 1;
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            mockUnitOfWork.Setup(u => u.whOrders.GetAll(
                It.IsAny<Expression<Func<whOrder, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(whOrders);


            // Act
            var result = orderService.GetOrders();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expecteduserOrders);
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
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            mockUnitOfWork.Setup(u => u.uOrders.GetAll(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(userOrders);
            

            // Act
            var result = orderService.GetuOrders();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expecteduserOrders);
        }

        [Fact]
        public void GetUserOrders_DeliveryOrders()
        {
            // Arrange
            DateTime datatime = DateTime.Now;
            var userOrders = new List<uOrder>
             {
                 new uOrder { Id = 1, Address = "TestAddress", Quantity = 100, Status = "ready", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 }, User = new User { Id = 1, Login = "TestName", } },
                 new uOrder { Id = 2, Address = "TestAddress", Quantity = 100, Status = "ready", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 }, User = new User { Id = 1, Login = "TestName", } },
             };

            var expecteduserOrders = new List<uOrderDTO>
             {
                 new uOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "ready", Date = datatime, Sum = 1000, ProductId = 1},
                 new uOrderDTO { Id = 2, Address = "TestAddress", Quantity = 100, Status = "ready", Date = datatime, Sum = 1000, ProductId = 1},
             };

            int? id = 1;
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            mockUnitOfWork.Setup(u => u.uOrders.GetAll(
                It.IsAny<Expression<Func<uOrder, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(userOrders);


            // Act
            var result = orderService.GetuOrders("delivery");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expecteduserOrders);
        }

        [Fact]
        public void SearchwhOrder_AllCorrect()
        {
            // Arrange
            DateTime datatime = DateTime.Now;
            var whOrders = new List<whOrder>
             {
                 new whOrder { Id = 1, Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 } },
                 new whOrder { Id = 11, Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1, Product = new Product{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 } },
             };

            var expectedwhOrders = new List<whOrderDTO>
             {
                 new whOrderDTO { Id = 1, Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1},
                 new whOrderDTO { Id = 11,  Quantity = 100, Status = "test", Date = datatime, Sum = 1000, ProductId = 1},
             };

            int? id = 1;
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);
            mockUnitOfWork.Setup(u => u.whOrders.GetAll(
                u => u.Id.ToString().Contains(id.ToString()),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
                )).Returns(whOrders);


            // Act
            var result = orderService.SearchwhOrder(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedwhOrders);
        }

        [Fact]
        public void Dispose_Service()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            IwhOrderService orderService = new whOrderService(mockUnitOfWork.Object);

            // Act
            orderService.Dispose();

            // Assert
            mockUnitOfWork.Verify(uow => uow.Dispose(), Times.Once);
        }
    }
}