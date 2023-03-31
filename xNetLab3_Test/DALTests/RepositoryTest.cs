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
    public class RepositoryTest
    {
        [Fact]
        public void GetAll_ReturnsCorrectEntities()
        {
            // Arrange
            var products = GetSampleProducts();

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Product>()).Returns(GetMockSet(products).Object);

            var repository = new ProductRepository(mockContext.Object);

            // Act
            var result = repository.GetAll();

            // Assert
            result.Should().HaveCount(3);

            result.Should().Contain(e => e.Id == 1 && e.Name == "Entity1");
            result.Should().Contain(e => e.Id == 2 && e.Name == "Entity2");
            result.Should().Contain(e => e.Id == 3 && e.Name == "Entity3");
        }

        [Fact]
        public void GetAll_FilterIsProvided_ReturnsFilteredData()
        {
            // Arrange
            var products = GetSampleProducts();

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Product>()).Returns(GetMockSet(products).Object);

            var repository = new ProductRepository(mockContext.Object);
            Expression<Func<Product, bool>> filter = e => e.Name == "Entity1";

            // Act
            var result = repository.GetAll(filter);

            // Assert
            result.Should().NotBeNullOrEmpty().And.HaveCount(1);
            result.Should().ContainSingle(p => p.Name == "Entity1");
        }

        [Fact]
        public void GetAll_WithIncludes_ReturnsCorrectEntities()
        {
            // Arrange
            var productsWithQuantity = GetSampleProductsIncludeQuantity();

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Product>()).Returns(GetMockSet(productsWithQuantity).Object);

            var repository = new ProductRepository(mockContext.Object);

            // Act
            var entities = repository.GetAll(includeProperties: "ProductQuantity");

            // Assert
            entities.Should().HaveCount(4);
            entities.Should().AllSatisfy(x =>
            {
                x.Id.Should().Be(x.ProductQuantity.ProductId);
            });
        }

        [Fact]
        public void GetAll_ReturnsCorrectEntitiesAmount2()
        {
            // Arrange
            var products = GetSampleProducts();

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Product>()).Returns(GetMockSet(products).Object);

            var repository = new ProductRepository(mockContext.Object);

            // Act
            var result = repository.GetAll(amount: 2);

            // Assert
            result.Should().HaveCount(2);

            result.Should().Contain(e => e.Id == 1 && e.Name == "Entity1");
            result.Should().Contain(e => e.Id == 2 && e.Name == "Entity2");
        }

        [Fact]
        public void TestGetFirstOrDefault()
        {
            // Arrange
            var products = GetSampleProducts();

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Product>()).Returns(GetMockSet(products).Object);

            var repository = new ProductRepository(mockContext.Object);

            // Act
            var product = repository.GetFirstOrDefault(p => p.Name == "Entity2");

            // Assert
            product.Should().NotBeNull();
            product.Id.Should().Be(2);
            product.Name.Should().Be("Entity2");
        }

        [Fact]
        public void TestGetFirstOrDefaultl_WithIncludes_ReturnsCorrectEntities()
        {
            // Arrange
            var date = DateTime.Now;
            var data = new List<uOrder>
            {
                new uOrder { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Date = date, Sum = 1000, ProductId = 1, UserId = 1, Product = new Product { Id = 1, Name = "TestProduct", Price = 100 }, User = new User { Id = 1, Login = "TestName1"} },
                new uOrder { Id = 2, Address = "TestAddress", Quantity = 100, Status = "test", Date = date, Sum = 1000, ProductId = 11, UserId = 2, Product = new Product { Id = 11, Name = "TestProduct", Price = 100 }, User = new User { Id = 2, Login = "TestName2"} },
                new uOrder { Id = 3, Address = "TestAddress", Quantity = 100, Status = "test", Date = date, Sum = 1000, ProductId = 1, UserId = 3, Product = new Product { Id = 1, Name = "TestProduct", Price = 100 }, User = new User { Id = 3, Login = "TestName3"} },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<uOrder>>();
            mockSet.As<IQueryable<uOrder>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<uOrder>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<uOrder>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<uOrder>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<uOrder>()).Returns(mockSet.Object);

            var repository = new Repository<uOrder>(mockContext.Object);

            // Act
            var order = repository.GetFirstOrDefault(p => p.UserId == 2, includeProperties: "Product,User");

            // Assert
            order.Should().NotBeNull();
            order.Id.Should().Be(2);
            order.UserId.Should().Be(order.User.Id).And.Be(2);
            order.ProductId.Should().Be(order.Product.Id).And.Be(11);
        }

        [Fact]
        public void TestGetFirstOrDefaultl_WithIncludesAsNoTracked_ReturnsCorrectEntities()
        {
            // Arrange
            var uOrders = GetSampleuOrdersIncludeProductAndUser();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<uOrder>()).Returns(GetMockSet(uOrders).Object);

            var repository = new Repository<uOrder>(mockContext.Object);

            // Act
            var order = repository.GetFirstOrDefault(p => p.UserId == 2, includeProperties: "Product,User", tracked: false);

            // Assert
            order.Should().NotBeNull();
            order.Id.Should().Be(2);
            order.UserId.Should().Be(order.User.Id).And.Be(2);
            order.ProductId.Should().Be(order.Product.Id).And.Be(11);
        }

        [Fact]
        public void Add_ShouldAddEntityToDbSet()
        {
            // Arrange
            var mockDbContext = new Mock<ApplicationDbContext>();
            var mockDbSet = new Mock<DbSet<whOrder>>();
            mockDbContext.Setup(x => x.Set<whOrder>()).Returns(mockDbSet.Object);
            var repository = new Repository<whOrder>(mockDbContext.Object);

            var entity = new whOrder();

            // Act
            repository.Add(entity);

            // Assert
            mockDbSet.Verify(x => x.Add(entity), Times.Once);
        }

        [Fact]
        public void Remove_ShouldRemoveEntityToDbSet()
        {
            // Arrange
            var mockDbContext = new Mock<ApplicationDbContext>();
            var mockDbSet = new Mock<DbSet<whOrder>>();
            mockDbContext.Setup(x => x.Set<whOrder>()).Returns(mockDbSet.Object);
            var repository = new Repository<whOrder>(mockDbContext.Object);

            var entity = new whOrder();

            // Act
            repository.Remove(entity);

            // Assert
            mockDbSet.Verify(x => x.Remove(entity), Times.Once);
        }

        [Fact]
        public void RemoveRange_RemovesEntitiesFromContext()
        {
            // Arrange
            var entities = new List<whOrder>
            {
                new whOrder { Id = 1 },
                new whOrder { Id = 2 },
                new whOrder { Id = 3 }
            };
            var mockDbSet = new Mock<DbSet<whOrder>>();
            mockDbSet.Setup(d => d.RemoveRange(entities));

            var mockDbContext = new Mock<ApplicationDbContext>();
            mockDbContext.Setup(c => c.Set<whOrder>()).Returns(mockDbSet.Object);

            var repository = new Repository<whOrder>(mockDbContext.Object);

            // Act
            repository.RemoveRange(entities);

            // Assert
            mockDbSet.Verify(d => d.RemoveRange(entities), Times.Once());
        }



        private Mock<DbSet<T>> GetMockSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }

        private IQueryable<Product> GetSampleProducts()
        {
            var data = new List<Product>
            {
                new Product { Id = 1, Name = "Entity1" },
                new Product { Id = 2, Name = "Entity2" },
                new Product { Id = 3, Name = "Entity3" }
            }.AsQueryable();
            return data;
        }
        private IQueryable<Product> GetSampleProductsIncludeQuantity()
        {
            var data = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", ProductQuantity = new ProductQuantity { Id = 1, Quantity = 1, ProductId = 1, ReservedQuantity = 1 } },
                new Product { Id = 2, Name = "Product 2", ProductQuantity = new ProductQuantity { Id = 2, Quantity = 1, ProductId = 2, ReservedQuantity = 1 } },
                new Product { Id = 3, Name = "Product 3", ProductQuantity = new ProductQuantity { Id = 3, Quantity = 1, ProductId = 3, ReservedQuantity = 1 } },
                new Product { Id = 4, Name = "Product 4", ProductQuantity = new ProductQuantity { Id = 4, Quantity = 1, ProductId = 4, ReservedQuantity = 1 } }
            }.AsQueryable();
            return data;
        }

        private IQueryable<uOrder> GetSampleuOrdersIncludeProductAndUser()
        {
            var date = DateTime.Now;
            var data = new List<uOrder>
            {
                new uOrder { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Date = date, Sum = 1000, ProductId = 1, UserId = 1, Product = new Product { Id = 1, Name = "TestProduct", Price = 100 }, User = new User { Id = 1, Login = "TestName1"} },
                new uOrder { Id = 2, Address = "TestAddress", Quantity = 100, Status = "test", Date = date, Sum = 1000, ProductId = 11, UserId = 2, Product = new Product { Id = 11, Name = "TestProduct", Price = 100 }, User = new User { Id = 2, Login = "TestName2"} },
                new uOrder { Id = 3, Address = "TestAddress", Quantity = 100, Status = "test", Date = date, Sum = 1000, ProductId = 1, UserId = 3, Product = new Product { Id = 1, Name = "TestProduct", Price = 100 }, User = new User { Id = 3, Login = "TestName3"} },
            }.AsQueryable();
            return data;
        }
    }


}