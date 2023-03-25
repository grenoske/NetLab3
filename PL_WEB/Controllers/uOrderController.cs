using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BLL.Interfaces;
using BLL.DTO;
using BLL.Infrastructure;
using PL_WEB.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PL_WEB.Util;

namespace PL_WEB.Controllers
{
    [Authorize]
    public class uOrderController : Controller
    {
        IuOrderService _orderService;
        IMyCookieManager _cookieManager;

        public uOrderController(IuOrderService orderService, IMyCookieManager myCookieManager)
        {
            _orderService = orderService;
            _cookieManager = myCookieManager;
        }

        public IActionResult Index(int pageA = 1)
        {
            
            int page = _cookieManager.PageMove("PageNumber" + nameof(Index), pageA, HttpContext.Request, HttpContext.Response);

            int id = Int32.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            IEnumerable<uOrderDTO> objUOrdersList = _orderService.GetUserOrders(id, page);
            var objUOrdersViewList = objUOrdersList.Select(uOrders => (uOrderViewModel)uOrders);
            return View(objUOrdersViewList);
        }
        public IActionResult ProductList(int pageA = 1)
        {
            int page = _cookieManager.PageMove("PageNumber" + nameof(ProductList), pageA, HttpContext.Request, HttpContext.Response);
          
            IEnumerable<ProductDTO> objProductList = _orderService.GetProducts(page);
            var objProductViewList = objProductList.Select(products => (ProductViewModel)products);
            return View(objProductViewList);
        }


        public IActionResult ProductSearch(string Name)
        {
            if(Name == null)
                return RedirectToAction(nameof(ProductList));
            IEnumerable<ProductDTO> objProductList = _orderService.SearchProduct(Name);
            var objProductViewList = objProductList.Select(products => (ProductViewModel)products);
            return View("ProductList", objProductViewList);
        }

        [Authorize(Roles = "admin")]
        public IActionResult uOrderSearch(int? id)
        {
            if (id == null)
                return RedirectToAction("AllUserOrders", "whOrder");
            IEnumerable<uOrderDTO> objuOrderList = _orderService.SearchuOrder(id);
            var objOrdersViewList = objuOrderList.Select(orders => (uOrderViewModel)orders);
            return View("../whOrder/AllUserOrders", objOrdersViewList);
        }

        public IActionResult UserOrderSearch(int? id)
        {
            int UserId = Int32.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id == null)
                return RedirectToAction(nameof(Index));
            IEnumerable<uOrderDTO> objuOrderList = _orderService.SearchUserOrder(id, UserId);
            var objOrdersViewList = objuOrderList.Select(orders => (uOrderViewModel)orders);
            return View("Index", objOrdersViewList);
        }

        public IActionResult ProductAdd(int id)
        {
            ProductViewModel objProduct = new ProductViewModel();
            if (id != 0)
            {
                try
                {
                    objProduct = (ProductViewModel)_orderService.GetProduct(id);
                }
                catch (ValidationException ex)
                {
                    return Content(ex.Message);
                }
            }
            return View(objProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProductAdd(ProductViewModel objProductModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _orderService.AddProduct(new ProductDTO { Name = objProductModel.Name, Company = objProductModel.Company, Price = objProductModel.Price });
                }
                catch(ValidationException ex)
                {
                    ViewBag.Message = (ex.Message);
                    return View(objProductModel);
                }
                return RedirectToAction(nameof(ProductList));
            }
            return View(objProductModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProductDelete(int id)
        {
            try
            {
                _orderService.RemoveProduct(id);
            }
            catch (ValidationException ex)
            {
                return Content(ex.Message);
            }

            return RedirectToAction(nameof(ProductList));
        }

        public IActionResult ProductDetail(int? id)
        {
            try
            {
                DetailProductDTO product = _orderService.GetDetailProduct(id);
                return View((DetailProductViewModel)product);
            }
            catch (ValidationException ex)
            {
                return Content(ex.Message);
            }
            
        }

        public IActionResult uOrderDetail(int? id)
        {
            try
            {
                DetailuOrderDTO order = _orderService.GetDetailuOrder(id);
                return View((DetailuOrderViewModel)order);
            }
            catch (ValidationException ex)
            {
                return Content(ex.Message);
            }

        }

        public IActionResult uOrderCancel(int id)
        {
            try
            {
                _orderService.CanceluOrder(id);
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                return Content(ex.Message);
            }
        }

        public IActionResult MakeOrder(int? id)
        {
            int UserId = Int32.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                ProductDTO product = _orderService.GetProduct(id);
                var order = new uOrderViewModel { ProductId = product.Id, UserId = UserId };

                return View(order);
            }
            catch (ValidationException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeOrder(uOrderViewModel order)
        {
            try
            {
                var orderDto = new uOrderDTO { UserId = order.UserId, ProductId = order.ProductId, Address = order.Address, Quantity = order.Quantity };
                _orderService.MakeOrder(orderDto);
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
            }
            return View(order);
        }
    }
}
