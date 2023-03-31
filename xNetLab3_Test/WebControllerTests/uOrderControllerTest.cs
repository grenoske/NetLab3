using Moq;
using FluentAssertions;
using BLL.Infrastructure;
using BLL.Services;
using BLL.Interfaces;
using BLL.DTO;
using PL_WEB.Util;
using PL_WEB.Controllers;
using Microsoft.AspNetCore.Mvc;
using PL_WEB.Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace xNetLab3_Test
{
    public class uOrderControllerTest
    {
        private Mock<IMyCookieManager> mockCookie;
        private Mock<IuOrderService> mockOrderService;
        private uOrderController uOrderController;
        public uOrderControllerTest()
        {
            mockCookie = new Mock<IMyCookieManager>();
            mockCookie.Setup(c => c.PageMove(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>())).Returns(1);

            mockOrderService = new Mock<IuOrderService>();

            uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);
        }

        [Fact]
        public void Index_ReturnsViewWithCorrectData()
        {
            // Arrange
            mockOrderService.Setup(s => s.GetUserOrders(1, It.IsAny<int>(), It.IsAny<int>())).Returns(GetSampleuOrdersDTO);
            uOrderController.ControllerContext = new ControllerContext { HttpContext = CreateMockHttpContext() };

            // Act
            var result = uOrderController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<uOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(GetSampleuOrdersDTO().Select(p => (uOrderViewModel)p));
        }

        [Fact]
        public void ProductSearch_ReturnsViewWithCorrectData()
        {
            // Arrange
            mockOrderService.Setup(s => s.SearchProduct("TestProduct")).Returns(GetSampleProductDTO);

            string prodName = "TestProduct";

            // Act
            var result = uOrderController.ProductSearch(prodName);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewName.Should().Be("ProductList");
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<ProductViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(GetSampleProductDTO().Select(p => (ProductViewModel)p));
        }


        [Fact]
        public void ProductSearch_NameNull_RedirectToAction()
        {
            // Arrange
            mockOrderService.Setup(s => s.SearchProduct("TestProduct")).Returns(GetSampleProductDTO);

            string prodName = null; 

            // Act
            var result = uOrderController.ProductSearch(prodName);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(uOrderController.ProductList));
        }

        [Fact]
        public void uOrderSearch_ALLCorrect()
        {
            // Arrange
            int? id = 1;
            mockOrderService.Setup(s => s.SearchuOrder(id)).Returns(GetSampleuOrdersDTO);

            // Act
            var result = uOrderController.uOrderSearch(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewName.Should().Be("../whOrder/AllUserOrders");
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<uOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(GetSampleuOrdersDTO().Select(p => (uOrderViewModel)p));
        }

        [Fact]
        public void uOrderSearch_IdNull_RedirectToAction()
        {
            // Arrange
            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);
            int? id = null;

            // Act
            var result = uOrderController.uOrderSearch(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            result.As<RedirectToActionResult>().ActionName.Should().Be("AllUserOrders", "whOrder");
        }

        [Fact]
        public void UserOrderSearch_IdNull_RedirectToAction()
        {
            // Arrange
            uOrderController.ControllerContext = new ControllerContext { HttpContext = CreateMockHttpContext() };

            int? id = null;

            // Act
            var result = uOrderController.UserOrderSearch(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(uOrderController.Index));
        }

        [Fact]
        public void UserOrderSearch_AllCorrect()
        {
            // Arrange
            int? id = 1;

            mockOrderService.Setup(s => s.SearchUserOrder(id, 1)).Returns(GetSampleuOrdersDTO);

            uOrderController.ControllerContext = new ControllerContext { HttpContext = CreateMockHttpContext() };

            // Act
            var result = uOrderController.UserOrderSearch(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewName.Should().Be("Index");
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<uOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(GetSampleuOrdersDTO().Select(p => (uOrderViewModel)p));
        }

        [Fact]
        public void ProductList_ReturnsViewWithCorrectData()
        {
            // Arrange
            mockOrderService.Setup(s => s.GetProducts(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(GetSampleProductDTO);

            uOrderController.ControllerContext = new ControllerContext { HttpContext = CreateMockHttpContext() };

            // Act
            var result = uOrderController.ProductList();

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<ProductViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(GetSampleProductDTO().Select(p => (ProductViewModel)p));
        }

        [Fact]
        public void ProductAdd_ReturnsViewWithCorrectData()
        {
            // Arrange
            int productId = 1;
            var product = new ProductDTO { Id = productId, Name = "TestProduct", Company = "TestCompany", Price = 100 };
            mockOrderService.Setup(s => s.GetProduct(It.IsAny<int>())).Returns(product);

            // Act
            var result = uOrderController.ProductAdd(productId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeOfType<ProductViewModel>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(product);
        }

        [Fact]
        public void ProductAdd_ReturnsError()
        {
            // Arrange
            int productId = 1;
       
            mockOrderService.Setup(s => s.GetProduct(It.IsAny<int>())).Throws(new ValidationException("Product not Found", ""));

            // Act
            var result = uOrderController.ProductAdd(productId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Product not Found");
        }

        [Fact]
        public void ProductAdd_ProductViewModel_ReturnsViewWithCorrectData()
        {
            // Arrange
            var product = new ProductViewModel { Name = "TestProduct", Company = "TestCompany", Price = 100 };

            // Act
            var result = uOrderController.ProductAdd(product);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(uOrderController.ProductList));
        }

        [Fact]
        public void ProductAdd_ProductViewModel_ReturnsError()
        {
            // Arrange
            var product = new ProductViewModel { Name = "TestProduct", Company = "TestCompany", Price = 100 };
            mockOrderService.Setup(s => s.AddProduct(It.IsAny<ProductDTO>())).Throws(new ValidationException("Product is already exists", ""));

            // Act
            var result = uOrderController.ProductAdd(product);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeOfType<ProductViewModel>();
            string str = uOrderController.ViewBag.Message;
            str.Should().Be("Product is already exists");
            
        }

        [Fact]
        public void ProductAdd_ProductViewModelisNotValid_Returns()
        {
            // Arrange
            ProductViewModel product = new ProductViewModel { Name = "", Company = "TestCompany", Price = 100 };
         
            uOrderController.ModelState.AddModelError("Name", "Name field is empty");

            // Act
            var result = uOrderController.ProductAdd(product);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeOfType<ProductViewModel>();
        }

        [Fact]
        public void ProductDelete_ReturnsError()
        {
            // Arrange
            int productId = 1;

            mockOrderService.Setup(s => s.RemoveProduct(It.IsAny<int>())).Throws(new ValidationException("Product not Found", ""));

            // Act
            var result = uOrderController.ProductDelete(productId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Product not Found");
        }

        [Fact]
        public void ProductDelete_Correct()
        {
            // Arrange
            int productId = 1;

            // Act
            var result = uOrderController.ProductDelete(productId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(uOrderController.ProductList));
        }

        [Fact]
        public void ProductDetail_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int validId = 1;
            DetailProductDTO detailProductDTO = new DetailProductDTO { Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100, Quantity = 100, ReservedQuantity = 100 };
            mockOrderService.Setup(x => x.GetDetailProduct(validId)).Returns(detailProductDTO);

            // Act
            var result = uOrderController.ProductDetail(validId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeOfType<DetailProductViewModel>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(detailProductDTO);
        }

        [Fact]
        public void ProductDetail_WithInvalidId_ReturnsContentResult()
        {
            // Arrange
            int invalidId = -1;
            mockOrderService.Setup(x => x.GetDetailProduct(invalidId)).Throws(new ValidationException("Product not Found", ""));

            // Act
            var result = uOrderController.ProductDetail(invalidId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Product not Found");
        }

        [Fact]
        public void uOrderDetail_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int validId = 1;
            DetailuOrderDTO detailuOrderDTO = new DetailuOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Sum = 1000, ProductId = 1, ProductName = "TestProduct", ProductPrice = 100, UserId = 1, UserLogin = "TestName" };
            mockOrderService.Setup(x => x.GetDetailuOrder(validId)).Returns(detailuOrderDTO);

            // Act
            var result = uOrderController.uOrderDetail(validId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeOfType<DetailuOrderViewModel>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(detailuOrderDTO);
        }

        [Fact]
        public void uOrderDetail_WithInvalidId_ReturnsContentResult()
        {
            // Arrange
            int invalidId = -1;
            mockOrderService.Setup(x => x.GetDetailuOrder(invalidId)).Throws(new ValidationException("uOrder not Found", ""));

            // Act
            var result = uOrderController.uOrderDetail(invalidId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("uOrder not Found");
        }

        [Fact]
        public void MakeOrder_ProductNotFound_ReturnsContentResultWithErrorMessage()
        {
            // Arrange
            int? productId = 1;
            mockOrderService.Setup(x => x.GetProduct(productId))
                .Throws(new ValidationException("Product not Found", ""));

            uOrderController.ControllerContext = new ControllerContext { HttpContext = CreateMockHttpContext() };

            // Act
            var result = uOrderController.MakeOrder(productId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Product not Found");
        }

        [Fact]
        public void MakeOrder_ProductFound_ReturnsViewResultWithOrderViewModel()
        {
            // Arrange
            int? productId = 1;
            var product = new ProductDTO { Id = productId.Value };

            mockOrderService.Setup(x => x.GetProduct(productId)).Returns(product);

            uOrderController.ControllerContext = new ControllerContext { HttpContext = CreateMockHttpContext() };

            // Act
            var result = uOrderController.MakeOrder(productId);

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<uOrderViewModel>()
                .Which.ProductId.Should().Be(productId.Value);

            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<uOrderViewModel>()
                .Which.UserId.Should().Be(1);
        }

        [Fact]
        public void MakeOrder_WithInvalidModel_ShouldAddModelErrorToModelState()
        {
            // Arrange
            mockOrderService.Setup(s => s.MakeOrder(It.IsAny<uOrderDTO>())).Throws(new ValidationException("Quantity cannot be less than Zero", "Quantity"));
            var invalidOrder = new uOrderViewModel { Quantity = -1 }; 

            // Act
            var result = uOrderController.MakeOrder(invalidOrder);

            // Assert
            result.As<ViewResult>().ViewName.Should().BeNullOrEmpty(); // Ensure that the view name is not set explicitly
            result.As<ViewResult>().Model.Should().BeSameAs(invalidOrder); // впенитися що неправильний обєкт повернен до вью
            uOrderController.ModelState.IsValid.Should().BeFalse(); // впевнитися що модель невірна
            uOrderController.ModelState.ErrorCount.Should().Be(1); // впевнится шо помилка 1
            uOrderController.ModelState.Keys.Should().Contain("Quantity"); 
            uOrderController.ModelState["Quantity"].Errors.Should().HaveCount(1); 
            uOrderController.ModelState["Quantity"].Errors[0].ErrorMessage.Should().Be("Quantity cannot be less than Zero"); 
        }

        [Fact]
        public void MakeOrder_WhenOrderIsValid_ShouldRedirectToIndex()
        {
            // Arrange
            var order = new uOrderViewModel { UserId = 1, Address = "Address 1", Quantity = 2 };
            var orderDto = new uOrderDTO { UserId = order.UserId, Address = order.Address, Quantity = order.Quantity };
            mockOrderService.Setup(x => x.MakeOrder(orderDto));

            // Act
            var result = uOrderController.MakeOrder(order);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(uOrderController.Index));
        }

        [Fact]
        public void uOrderCancel_OrderCancelled_RedirectsToIndex()
        {
            // Arrange
            mockOrderService.Setup(os => os.CanceluOrder(It.IsAny<int>()));

            // Act
            var result = uOrderController.uOrderCancel(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(uOrderControllerTest.uOrderController.Index));

            mockOrderService.Verify(os => os.CanceluOrder(1), Times.Once);
        }

        [Fact]
        public void uOrderCancel_ValidationException_ReturnsContent()
        {
            // Arrange
            mockOrderService
                .Setup(os => os.CanceluOrder(It.IsAny<int>()))
                .Throws(new ValidationException("Order not found", ""));

            // Act
            var result = uOrderController.uOrderCancel(1);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Order not found");
        }

        private HttpContext CreateMockHttpContext()
        {
            var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.User).Returns(principal);
            mockHttpContext.Setup(x => x.Request).Returns(new Mock<HttpRequest>().Object);
            mockHttpContext.Setup(x => x.Response).Returns(new Mock<HttpResponse>().Object);

            return mockHttpContext.Object;
        }
        private IEnumerable<ProductDTO> GetSampleProductDTO()
        {
            IEnumerable<ProductDTO> products = new List<ProductDTO>
            {
                new ProductDTO{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },
                new ProductDTO{ Id = 2, Name = "TestProduct2", Company = "TestCompany2", Price = 100 },
                new ProductDTO{ Id = 3, Name = "TestProduct3", Company = "TestCompany3", Price = 100 }
            };
            return products;
        }
        private IEnumerable<uOrderDTO> GetSampleuOrdersDTO()
        {
            IEnumerable<uOrderDTO> uOrders = new List<uOrderDTO>
            {
                new uOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Sum = 1000, ProductId = 1},
                new uOrderDTO { Id = 11, Address = "TestAddress", Quantity = 100, Status = "test", Sum = 1000, ProductId = 1},
            };
            return uOrders;
        }
    }
}