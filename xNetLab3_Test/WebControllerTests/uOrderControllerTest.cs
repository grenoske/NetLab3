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
        [Fact]
        public void Index_ReturnsViewWithCorrectData()
        {
            // Arrange
            var uOrders = new List<uOrderDTO>
             {
                 new uOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Sum = 1000, ProductId = 1},
                 new uOrderDTO { Id = 2, Address = "TestAddress", Quantity = 100, Status = "test", Sum = 1000, ProductId = 1},
             };

            var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.User).Returns(principal);
            mockHttpContext.Setup(x => x.Request).Returns(new Mock<HttpRequest>().Object);
            mockHttpContext.Setup(x => x.Response).Returns(new Mock<HttpResponse>().Object);

            var mockCookie = new Mock<IMyCookieManager>();
            mockCookie.Setup(c => c.PageMove(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>())).Returns(1);

            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.GetUserOrders(1, It.IsAny<int>(), It.IsAny<int>())).Returns(uOrders);

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };
      

            // Act
            var result = uOrderController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<uOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(uOrders.Select(p => (uOrderViewModel)p));
        }

        [Fact]
        public void ProductSearch_ReturnsViewWithCorrectData()
        {
            // Arrange
            IEnumerable<ProductDTO> products = new List<ProductDTO>
            {
                new ProductDTO{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },
                new ProductDTO{ Id = 2, Name = "TestProduct2", Company = "TestCompany2", Price = 100 },
                new ProductDTO{ Id = 3, Name = "TestProduct3", Company = "TestCompany3", Price = 100 }
            };

            var mockCookie = new Mock<IMyCookieManager>();

            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.SearchProduct("TestProduct")).Returns(products);

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

            string prodName = "TestProduct";


            // Act
            var result = uOrderController.ProductSearch(prodName);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewName.Should().Be("ProductList");
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<ProductViewModel>>();
            //result.As<ViewResult>().Model.Should().HaveSameCount(products);
            result.As<ViewResult>().Model.Should().BeEquivalentTo(products.Select(p => (ProductViewModel)p));
        }

        [Fact]
        public void ProductSearch_NameNull_RedirectToAction()
        {
            // Arrange
            IEnumerable<ProductDTO> products = new List<ProductDTO>
            {
                new ProductDTO{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },
                new ProductDTO{ Id = 2, Name = "TestProduct2", Company = "TestCompany2", Price = 100 },
                new ProductDTO{ Id = 3, Name = "TestProduct3", Company = "TestCompany3", Price = 100 }
            };

            var mockCookie = new Mock<IMyCookieManager>();

            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.SearchProduct("TestProduct")).Returns(products);

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

            string prodName = null; ;


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
            IEnumerable<uOrderDTO> uOrders = new List<uOrderDTO>
            {
                new uOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Sum = 1000, ProductId = 1},
                new uOrderDTO { Id = 11, Address = "TestAddress", Quantity = 100, Status = "test", Sum = 1000, ProductId = 1},
            };
            var mockCookie = new Mock<IMyCookieManager>();

            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.SearchuOrder(id)).Returns(uOrders);

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

            // Act
            var result = uOrderController.uOrderSearch(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewName.Should().Be("../whOrder/AllUserOrders");
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<uOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(uOrders.Select(p => (uOrderViewModel)p));
        }

        [Fact]
        public void uOrderSearch_IdNull_RedirectToAction()
        {
            // Arrange
            var mockCookie = new Mock<IMyCookieManager>();

            var mockOrderService = new Mock<IuOrderService>();

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
            var mockCookie = new Mock<IMyCookieManager>();
            var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.User).Returns(principal);

            var mockOrderService = new Mock<IuOrderService>();

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

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
            IEnumerable<uOrderDTO> uOrders = new List<uOrderDTO>
            {
                new uOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Sum = 1000, ProductId = 1},
                new uOrderDTO { Id = 11, Address = "TestAddress", Quantity = 100, Status = "test", Sum = 1000, ProductId = 1},
            };

            var mockCookie = new Mock<IMyCookieManager>();
            var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.User).Returns(principal);

            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.SearchUserOrder(id, 1)).Returns(uOrders);

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = uOrderController.UserOrderSearch(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewName.Should().Be("Index");
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<uOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(uOrders.Select(p => (uOrderViewModel)p));
        }

        [Fact]
        public void ProductList_ReturnsViewWithCorrectData()
        {
            // Arrange
            IEnumerable<ProductDTO> products = new List<ProductDTO>
            {
                new ProductDTO{ Id = 1, Name = "TestProduct", Company = "TestCompany", Price = 100 },
                new ProductDTO{ Id = 2, Name = "TestProduct2", Company = "TestCompany2", Price = 100 },
                new ProductDTO{ Id = 3, Name = "TestProduct3", Company = "TestCompany3", Price = 100 }
            };


            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(new Mock<HttpRequest>().Object);
            mockHttpContext.Setup(x => x.Response).Returns(new Mock<HttpResponse>().Object);

            var mockCookie = new Mock<IMyCookieManager>();
            mockCookie.Setup(c => c.PageMove(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>())).Returns(1);

            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.GetProducts(It.IsAny<int>(), It.IsAny<int>())).Returns(products);

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object) { ControllerContext = new ControllerContext() { HttpContext = mockHttpContext.Object } };

            // Act
            var result = uOrderController.ProductList();

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<ProductViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(products.Select(p => (ProductViewModel)p));
        }

        [Fact]
        public void ProductAdd_ReturnsViewWithCorrectData()
        {
            // Arrange
            int productId = 1;
            var product = new ProductDTO { Id = productId, Name = "TestProduct", Company = "TestCompany", Price = 100 };
            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.GetProduct(It.IsAny<int>())).Returns(product);

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

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
            
            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.GetProduct(It.IsAny<int>())).Throws(new ValidationException("Product not Found", ""));

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

            // Act
            var result = uOrderController.ProductAdd(productId);

            // Assert
            result.Should().BeOfType<ContentResult>()
                .Which.Content.Should().Be("Product not Found");
        }

        [Fact]
        public void ProductAdd_ProductViewModel_ReturnsViewWithCorrectData()
        {
            // Arrange
            var product = new ProductViewModel { Name = "TestProduct", Company = "TestCompany", Price = 100 };
            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

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
            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.AddProduct(It.IsAny<ProductDTO>())).Throws(new ValidationException("Product is already exists", ""));

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

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
            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            
            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);
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

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.RemoveProduct(It.IsAny<int>())).Throws(new ValidationException("Product not Found", ""));

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

            // Act
            var result = uOrderController.ProductDelete(productId);

            // Assert
            result.Should().BeOfType<ContentResult>()
                .Which.Content.Should().Be("Product not Found");
        }

        [Fact]
        public void ProductDelete_Correct()
        {
            // Arrange
            int productId = 1;

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

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

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(x => x.GetDetailProduct(validId)).Returns(detailProductDTO);

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

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

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(x => x.GetDetailProduct(invalidId)).Throws(new ValidationException("Product not Found", ""));

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

            // Act
            var result = uOrderController.ProductDetail(invalidId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ContentResult>()
                .Which.Content.Should().Be("Product not Found");
        }

        [Fact]
        public void uOrderDetail_WithValidId_ReturnsViewResult()
        {
            // Arrange
            int validId = 1;
            DetailuOrderDTO detailuOrderDTO = new DetailuOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "test", Sum = 1000, ProductId = 1, ProductName = "TestProduct", ProductPrice = 100, UserId = 1, UserLogin = "TestName" };

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(x => x.GetDetailuOrder(validId)).Returns(detailuOrderDTO);

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

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

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(x => x.GetDetailuOrder(invalidId)).Throws(new ValidationException("uOrder not Found", ""));

            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object);

            // Act
            var result = uOrderController.uOrderDetail(invalidId);

            // Assert
            result.Should().BeOfType<ContentResult>()
                .Which.Content.Should().Be("uOrder not Found");
        }

        [Fact]
        public void MakeOrder_ProductNotFound_ReturnsContentResultWithErrorMessage()
        {
            // Arrange
            int? productId = 1;

            var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.User).Returns(principal);

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(x => x.GetProduct(productId))
                .Throws(new ValidationException("Product not Found", ""));


            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = uOrderController.MakeOrder(productId);

            // Assert
            result.Should().BeOfType<ContentResult>()
                .Which.Content.Should().Be("Product not Found");
        }

        [Fact]
        public void MakeOrder_ProductFound_ReturnsViewResultWithOrderViewModel()
        {
            // Arrange
            int? productId = 1;
            var product = new ProductDTO { Id = productId.Value };

            var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.User).Returns(principal);

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(x => x.GetProduct(productId)).Returns(product);


            uOrderController uOrderController = new uOrderController(mockOrderService.Object, mockCookie.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

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
            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(s => s.MakeOrder(It.IsAny<uOrderDTO>())).Throws(new ValidationException("Quantity cannot be less than Zero", "Quantity"));
            
            uOrderController controller = new uOrderController(mockOrderService.Object, mockCookie.Object);

            var invalidOrder = new uOrderViewModel { Quantity = -1 }; 

            // Act
            var result = controller.MakeOrder(invalidOrder);

            // Assert
            result.As<ViewResult>().ViewName.Should().BeNullOrEmpty(); // Ensure that the view name is not set explicitly
            result.As<ViewResult>().Model.Should().BeSameAs(invalidOrder); // впенитися що неправильний обєкт повернен до вью
            controller.ModelState.IsValid.Should().BeFalse(); // впевнитися що модель невірна
            controller.ModelState.ErrorCount.Should().Be(1); // впевнится шо помилка 1
            controller.ModelState.Keys.Should().Contain("Quantity"); 
            controller.ModelState["Quantity"].Errors.Should().HaveCount(1); 
            controller.ModelState["Quantity"].Errors[0].ErrorMessage.Should().Be("Quantity cannot be less than Zero"); 
        }

        [Fact]
        public void MakeOrder_WhenOrderIsValid_ShouldRedirectToIndex()
        {
            // Arrange
            var order = new uOrderViewModel { UserId = 1, Address = "Address 1", Quantity = 2 };
            var orderDto = new uOrderDTO { UserId = order.UserId, Address = order.Address, Quantity = order.Quantity };

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IuOrderService>();
            mockOrderService.Setup(x => x.MakeOrder(orderDto));

            uOrderController controller = new uOrderController(mockOrderService.Object, mockCookie.Object);


            // Act
            var result = controller.MakeOrder(order);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(controller.Index));
        }

        [Fact]
        public void uOrderCancel_OrderCancelled_RedirectsToIndex()
        {
            // Arrange
            var orderServiceMock = new Mock<IuOrderService>();
            orderServiceMock.Setup(os => os.CanceluOrder(It.IsAny<int>()));

            var MyCookieMock = new Mock<IMyCookieManager>();

            var controller = new uOrderController(orderServiceMock.Object, MyCookieMock.Object);

            // Act
            var result = controller.uOrderCancel(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(uOrderController.Index));

            orderServiceMock.Verify(os => os.CanceluOrder(1), Times.Once);
        }

        [Fact]
        public void uOrderCancel_ValidationException_ReturnsContent()
        {
            // Arrange
            var orderServiceMock = new Mock<IuOrderService>();
            orderServiceMock
                .Setup(os => os.CanceluOrder(It.IsAny<int>()))
                .Throws(new ValidationException("Order not found", ""));
            var MyCookieMock = new Mock<IMyCookieManager>();

            var controller = new uOrderController(orderServiceMock.Object, MyCookieMock.Object);

            // Act
            var result = controller.uOrderCancel(1);

            // Assert
            result.Should().BeOfType<ContentResult>()
                .Which.Content.Should().Be("Order not found");
        }
    }
}