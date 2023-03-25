using Moq;
using FluentAssertions;
using Xunit;
using DAL.Interfaces;
using DAL.Repositories;
using BLL.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Ninject;

namespace xNetLab3_Test
{
    public class ServiceModuleTests
    {
        [Fact]
        public void Load_ShouldBindIUnitOfWorkToUnitOfWork()
        {
            // Arrange
            var connectionString = "fakeConnectionString";
            var serviceModule = new ServiceModule(connectionString);

            var kernel = new StandardKernel(serviceModule);

            // Act
            var actual = kernel.Get<IUnitOfWork>();

            // Assert
            actual.Should().BeAssignableTo<UnitOfWork>();
        }
    }
}