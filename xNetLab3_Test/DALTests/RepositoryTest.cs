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
            var data = new List<Product>
            {
                new Product { Id = 1, Name = "Entity1" },
                new Product { Id = 2, Name = "Entity2" },
                new Product { Id = 3, Name = "Entity3" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Product>()).Returns(mockSet.Object);

            var repository = new Repository<Product>(mockContext.Object);

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
            var expectedProducts = new List<Product>()
            {
                new Product() { Id = 1, Name = "Entity1" },
                new Product() { Id = 2, Name = "Entity2" },
                new Product() { Id = 3, Name = "Entity3" }
            };

            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(expectedProducts.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(expectedProducts.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(expectedProducts.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(expectedProducts.GetEnumerator());
            var mockDbContext = new Mock<ApplicationDbContext>();
            
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new Repository<Product>(mockDbContext.Object);
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
            var data = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", ProductQuantity = new ProductQuantity { Id = 1, Quantity = 1, ProductId = 1, ReservedQuantity = 1 } },
                new Product { Id = 2, Name = "Product 2", ProductQuantity = new ProductQuantity { Id = 2, Quantity = 1, ProductId = 2, ReservedQuantity = 1 } },
                new Product { Id = 3, Name = "Product 3", ProductQuantity = new ProductQuantity { Id = 3, Quantity = 1, ProductId = 3, ReservedQuantity = 1 } },
                new Product { Id = 4, Name = "Product 4", ProductQuantity = new ProductQuantity { Id = 4, Quantity = 1, ProductId = 4, ReservedQuantity = 1 } }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Product>()).Returns(mockSet.Object);

            var repository = new Repository<Product>(mockContext.Object);

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
        public void GetAll_ReturnsCorrectEntities2()
        {
            // Arrange
            var data = new List<Product>
            {
                new Product { Id = 1, Name = "Entity1" },
                new Product { Id = 2, Name = "Entity2" },
                new Product { Id = 3, Name = "Entity3" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Product>()).Returns(mockSet.Object);

            var repository = new Repository<Product>(mockContext.Object);

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
            var data = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Price = 10 },
                new Product { Id = 2, Name = "Product2", Price = 20 },
                new Product { Id = 3, Name = "Product3", Price = 30 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Product>()).Returns(mockSet.Object);

            var repository = new Repository<Product>(mockContext.Object);

            // Act
            var product = repository.GetFirstOrDefault(p => p.Name == "Product2");

            // Assert
            product.Should().NotBeNull();
            product.Id.Should().Be(2);
            product.Name.Should().Be("Product2");
            product.Price.Should().Be(20);
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
            var order = repository.GetFirstOrDefault(p => p.UserId == 2, includeProperties:"Product,User");

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
            var order = repository.GetFirstOrDefault(p => p.UserId == 2, includeProperties: "Product,User", tracked: false);

            // Assert
            order.Should().NotBeNull();
            order.Id.Should().Be(2);
            order.UserId.Should().Be(order.User.Id).And.Be(2);
            order.ProductId.Should().Be(order.Product.Id).And.Be(11);

            //mockSet.Verify(x => x.AsNoTracking(), Times.Once);
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
    }
}