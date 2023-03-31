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
    public class whOrderControllerTest
    {
        [Fact]
        public void Index_ReturnsViewWithCorrectData()
        {
            // Arrange
            var whOrders = new List<whOrderDTO>
             {
                 new whOrderDTO { Id = 1, Quantity = 100, Status = "wait", Sum = 1000, ProductId = 1},
                 new whOrderDTO { Id = 2, Quantity = 100, Status = "wait", Sum = 1000, ProductId = 1},
             };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(new Mock<HttpRequest>().Object);
            mockHttpContext.Setup(x => x.Response).Returns(new Mock<HttpResponse>().Object);

            var mockCookie = new Mock<IMyCookieManager>();
            mockCookie.Setup(c => c.PageMove(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>())).Returns(1);

            var mockOrderService = new Mock<IwhOrderService>();
            mockOrderService.Setup(s => s.GetOrders(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(whOrders);

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object) { ControllerContext = new ControllerContext() { HttpContext = mockHttpContext.Object } };

            // Act
            var result = whOrderController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<whOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(whOrders.Select(p => (whOrderViewModel)p));
        }

        [Fact]
        public void PurchaseQueue_ReturnsViewWithCorrectData()
        {
            // Arrange
            var whOrders = new List<whOrderDTO>
             {
                 new whOrderDTO { Id = 1, Quantity = 100, Status = "wait", Sum = 1000, ProductId = 1},
                 new whOrderDTO { Id = 2, Quantity = 100, Status = "wait", Sum = 1000, ProductId = 1},
             };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(new Mock<HttpRequest>().Object);
            mockHttpContext.Setup(x => x.Response).Returns(new Mock<HttpResponse>().Object);

            var mockCookie = new Mock<IMyCookieManager>();
            mockCookie.Setup(c => c.PageMove(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>())).Returns(1);

            var mockOrderService = new Mock<IwhOrderService>();
            mockOrderService.Setup(s => s.GetOrders("purchase", It.IsAny<int>(), It.IsAny<int>())).Returns(whOrders);

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object) { ControllerContext = new ControllerContext() { HttpContext = mockHttpContext.Object } };

            // Act
            var result = whOrderController.PurchaseQueue();

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<whOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(whOrders.Select(p => (whOrderViewModel)p));
        }

        [Fact]
        public void DeliveryQueue_ReturnsViewWithCorrectData()
        {
            // Arrange
            var uOrders = new List<uOrderDTO>
             {
                 new uOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "ready", Sum = 1000, ProductId = 1},
                 new uOrderDTO { Id = 2, Address = "TestAddress", Quantity = 100, Status = "ready", Sum = 1000, ProductId = 1},
             };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(new Mock<HttpRequest>().Object);
            mockHttpContext.Setup(x => x.Response).Returns(new Mock<HttpResponse>().Object);

            var mockCookie = new Mock<IMyCookieManager>();
            mockCookie.Setup(c => c.PageMove(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>())).Returns(1);

            var mockOrderService = new Mock<IwhOrderService>();
            mockOrderService.Setup(s => s.GetuOrders("delivery", It.IsAny<int>(), It.IsAny<int>())).Returns(uOrders);

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = whOrderController.DeliveryQueue();

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<uOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(uOrders.Select(p => (uOrderViewModel)p));
        }

        [Fact]
        public void AllUserOrders_ReturnsViewWithCorrectData()
        {
            // Arrange
            var uOrders = new List<uOrderDTO>
             {
                 new uOrderDTO { Id = 1, Address = "TestAddress", Quantity = 100, Status = "ready", Sum = 1000, ProductId = 1},
                 new uOrderDTO { Id = 2, Address = "TestAddress", Quantity = 100, Status = "wait", Sum = 1000, ProductId = 1},
             };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.Request).Returns(new Mock<HttpRequest>().Object);
            mockHttpContext.Setup(x => x.Response).Returns(new Mock<HttpResponse>().Object);

            var mockCookie = new Mock<IMyCookieManager>();
            mockCookie.Setup(c => c.PageMove(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>())).Returns(1);

            var mockOrderService = new Mock<IwhOrderService>();
            mockOrderService.Setup(s => s.GetuOrders(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(uOrders);

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = whOrderController.AllUserOrders();

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<uOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(uOrders.Select(p => (uOrderViewModel)p));
        }

        [Fact]
        public void ProcessSpecificPurchase_InvalidId_ErrorReturn()
        {
            // Arrange
            int id = 1;

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IwhOrderService>();
            mockOrderService.Setup(x => x.DeliveryProductToWh(id)).Throws(new ValidationException("Product not Found", ""));
            
            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object);
            // Act
            var result = whOrderController.ProcessSpecificPurchase(id);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Product not Found");
        }

        [Fact]
        public void ProcessSpecificPurchase_ValidId_Correct()
        {
            // Arrange
            int id = 1;

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IwhOrderService>();

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object);
            // Act
            var result = whOrderController.ProcessSpecificPurchase(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(whOrderController.PurchaseQueue));

            mockOrderService.Verify(x => x.DeliveryProductToWh(1), Times.Once);
        }

        [Fact]
        public void ProcessNextPurchase_Correct()
        {
            // Arrange
            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IwhOrderService>();

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object);
            // Act
            var result = whOrderController.ProcessNextPurchase();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(whOrderController.PurchaseQueue));

            mockOrderService.Verify(x => x.DeliveryProductToWh(null), Times.Once);
        }

        [Fact]
        public void ProcessNextPurchase_ErrorReturn()
        {
            // Arrange
            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IwhOrderService>();
            mockOrderService.Setup(x => x.DeliveryProductToWh(null)).Throws(new ValidationException("Order not Found", ""));

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object);
            // Act
            var result = whOrderController.ProcessNextPurchase();

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
               .Which.Value.Should().Be("Order not Found", "");
        }

        [Fact]
        public void DeliverySpecificOrder_ErrorReturn()
        {
            // Arrange
            int id = 1;

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IwhOrderService>();
            mockOrderService.Setup(x => x.DeliveryProductToUser(id)).Throws(new ValidationException("User not Found", ""));

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object);
            // Act
            var result = whOrderController.DeliverySpecificOrder(id);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("User not Found");
        }

        [Fact]
        public void DeliverySpecificOrder_ValidId_Correct()
        {
            // Arrange
            int id = 1;

            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IwhOrderService>();

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object);
            // Act
            var result = whOrderController.DeliverySpecificOrder(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(whOrderController.DeliveryQueue));

            mockOrderService.Verify(x => x.DeliveryProductToUser(1), Times.Once);
        }

        [Fact]
        public void DeliveryNextOrder_Correct()
        {
            // Arrange
            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IwhOrderService>();

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object);
            // Act
            var result = whOrderController.DeliveryNextOrder();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(whOrderController.DeliveryQueue));

            mockOrderService.Verify(x => x.DeliveryProductToUser(null), Times.Once);
        }

        [Fact]
        public void DeliveryNextOrder_ErrorReturn()
        {
            // Arrange
            var mockCookie = new Mock<IMyCookieManager>();
            var mockOrderService = new Mock<IwhOrderService>();
            mockOrderService.Setup(x => x.DeliveryProductToUser(null)).Throws(new ValidationException("Order not Found", ""));

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object);
            // Act
            var result = whOrderController.DeliveryNextOrder();

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Order not Found");
                //.Which.object.Should().Be("Order not Found");
            //result.Should().BeOfType<NotFoundResult>(); // 404
        }

        [Fact]
        public void whOrderDetail_WithValidId_ReturnsViewWithModel()
        {
            // Arrange
            int validId = 1;
            var expectedOrder = new DetailwhOrderDTO { Id = 1, Quantity = 100, Status = "wait", Sum = 100, ProductId = 1, ProductName = "TestProduct", ProductCompany = "TestCompany", ProductPrice = 1 }; ;

            var mockCookie = new Mock<IMyCookieManager>();
            var mockService = new Mock<IwhOrderService>();
            mockService.Setup(s => s.GetDetailwhOrder(validId)).Returns(expectedOrder);

            whOrderController controller = new whOrderController(mockService.Object, mockCookie.Object);

            // Act
            var result = controller.whOrderDetail(validId);

            // Assert
            result.As<ViewResult>().Model.Should().BeOfType<DetailwhOrderViewModel>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedOrder);
        }
        
        [Fact]
        public void whOrderDetail_WithInvalidId_ReturnsContentWithErrorMessage()
        {
            // Arrange
            int invalidId = 0;
            string expectedMessage = "Invalid ID";

            var mockCookie = new Mock<IMyCookieManager>();
            var mockService = new Mock<IwhOrderService>();
            mockService.Setup(s => s.GetDetailwhOrder(invalidId)).Throws(new ValidationException(expectedMessage, ""));

            whOrderController controller = new whOrderController(mockService.Object, mockCookie.Object);

            // Act
            var result = controller.whOrderDetail(invalidId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be(expectedMessage);
        }

        [Fact]
        public void whOrderSearch_ALLCorrect()
        {
            // Arrange
            int? id = 1;
            IEnumerable<whOrderDTO> whOrders = new List<whOrderDTO>
            {
                new whOrderDTO { Id = 1,  Quantity = 100, Status = "test", Sum = 1000, ProductId = 1},
                new whOrderDTO { Id = 11,  Quantity = 100, Status = "test", Sum = 1000, ProductId = 1},
            };
            var mockCookie = new Mock<IMyCookieManager>();

            var mockOrderService = new Mock<IwhOrderService>();
            mockOrderService.Setup(s => s.SearchwhOrder(id)).Returns(whOrders);

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object);

            // Act
            var result = whOrderController.whOrderSearch(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewName.Should().Be("Index");
            result.As<ViewResult>().Model.Should().BeAssignableTo<IEnumerable<whOrderViewModel>>();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(whOrders.Select(p => (whOrderViewModel)p));
        }

        [Fact]
        public void uOrderSearch_IdNull_RedirectToAction()
        {
            // Arrange
            var mockCookie = new Mock<IMyCookieManager>();

            var mockOrderService = new Mock<IwhOrderService>();

            whOrderController whOrderController = new whOrderController(mockOrderService.Object, mockCookie.Object);

            int? id = null;


            // Act
            var result = whOrderController.whOrderSearch(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            result.As<RedirectToActionResult>().ActionName.Should().Be("Index", "whOrder");
        }

    }
}