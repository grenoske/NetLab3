using Moq;
using FluentAssertions;
using DAL.Repositories;
using DAL.Interfaces;
using DAL.EF;
using Microsoft.EntityFrameworkCore;
using DAL.Entities;
using NuGet.Protocol.Core.Types;
using System.Linq.Expressions;

namespace xNetLab3_Test
{

    public class UnitOfWorkTest
    {
        [Fact]
        public void Products_Should_Return_ProductRepository()
        {
            // Arrange
            var mockDbContext = new Mock<ApplicationDbContext>();
            var unitOfWork = new UnitOfWork(mockDbContext.Object);

            // Act
            var products = unitOfWork.Products;

            // Assert
            products.Should().BeOfType<ProductRepository>();
        }

        [Fact]
        public void uOrders_Should_Return_uOrderRepository()
        {
            // Arrange
            var mockDbContext = new Mock<ApplicationDbContext>();
            var unitOfWork = new UnitOfWork(mockDbContext.Object);

            // Act
            var uOrders = unitOfWork.uOrders;

            // Assert
            uOrders.Should().BeOfType<uOrderRepository>();
        }

        [Fact]
        public void Users_Should_Return_UserRepository()
        {
            // Arrange
            var mockDbContext = new Mock<ApplicationDbContext>();
            var unitOfWork = new UnitOfWork(mockDbContext.Object);

            // Act
            var users = unitOfWork.Users;

            // Assert
            users.Should().BeOfType<UserRepository>();
        }

        [Fact]
        public void whOrders_Should_Return_whOrderRepository()
        {
            // Arrange
            var mockDbContext = new Mock<ApplicationDbContext>();
            var unitOfWork = new UnitOfWork(mockDbContext.Object);

            // Act
            var whOrders = unitOfWork.whOrders;

            // Assert
            whOrders.Should().BeOfType<whOrderRepository>();
        }

        [Fact]
        public void Warehouse_Should_Return_WarehouseRepository()
        {
            // Arrange
            var mockDbContext = new Mock<ApplicationDbContext>();
            var unitOfWork = new UnitOfWork(mockDbContext.Object);

            // Act
            var warehouse = unitOfWork.Warehouse;

            // Assert
            warehouse.Should().BeOfType<WarehouseRepository>();
        }

        [Fact]
        public void ProductQuantity_Should_Return_ProductQuantityRepository()
        {
            // Arrange
            var mockDbContext = new Mock<ApplicationDbContext>();
            var unitOfWork = new UnitOfWork(mockDbContext.Object);

            // Act
            var productQuantity = unitOfWork.ProductQuantity;

            // Assert
            productQuantity.Should().BeOfType<ProductQuantityRepository>();
        }

        [Fact]
        public void Save_Should_Call_SaveChanges_On_DbContext()
        {
            // Arrange
            var mockDbContext = new Mock<ApplicationDbContext>();
            var unitOfWork = new UnitOfWork(mockDbContext.Object);

            // Act
            unitOfWork.Save();

            // Assert
            mockDbContext.Verify(x => x.SaveChanges(), Times.Once());
        }
    }
}