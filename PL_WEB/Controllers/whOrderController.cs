using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.DTO;
using BLL.Infrastructure;
using PL_WEB.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using PL_WEB.Util;

namespace PL_WEB.Controllers
{
    [Authorize(Roles = "admin")]
    public class whOrderController : Controller
    {
        IwhOrderService _whOrderService;
        IMyCookieManager _cookieManager;

        public whOrderController(IwhOrderService whOrderService, IMyCookieManager myCookieManager)
        {
            _whOrderService = whOrderService;
            _cookieManager = myCookieManager;
        }

        public IActionResult Index(int pageA = 1)
        {
            int page = _cookieManager.PageMove("PageNumber" + nameof(Index) + 2, pageA, HttpContext.Request, HttpContext.Response);

            IEnumerable<whOrderDTO> objWOrdersList = _whOrderService.GetOrders(null,page);
            var objWOrdersViewList = objWOrdersList.Select(uOrders => (whOrderViewModel)uOrders);
            return View(objWOrdersViewList);
        }

        public IActionResult PurchaseQueue(int pageA = 1)
        {
            int page = _cookieManager.PageMove("PageNumber" + nameof(PurchaseQueue), pageA, HttpContext.Request, HttpContext.Response);

            IEnumerable<whOrderDTO> objWOrdersList = _whOrderService.GetOrders("purchase", page);
            var objWOrdersViewList = objWOrdersList.Select(uOrders => (whOrderViewModel)uOrders);
            return View(objWOrdersViewList);
        }

        public IActionResult DeliveryQueue(int pageA = 1)
        {
            int page = _cookieManager.PageMove("PageNumber" + nameof(DeliveryQueue), pageA, HttpContext.Request, HttpContext.Response);

            IEnumerable<uOrderDTO> objWOrdersList = _whOrderService.GetuOrders("delivery", page);
            var objWOrdersViewList = objWOrdersList.Select(uOrders => (uOrderViewModel)uOrders);
            return View(objWOrdersViewList);
        }
        public IActionResult AllUserOrders(int pageA = 1)
        {
            int page = _cookieManager.PageMove("PageNumber" + nameof(AllUserOrders), pageA, HttpContext.Request, HttpContext.Response);

            IEnumerable<uOrderDTO> objWOrdersList = _whOrderService.GetuOrders(null, page);
            var objWOrdersViewList = objWOrdersList.Select(uOrders => (uOrderViewModel)uOrders);
            return View(objWOrdersViewList);
        }

        public IActionResult whOrderSearch(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "whOrder");
            IEnumerable<whOrderDTO> objuOrderList = _whOrderService.SearchwhOrder(id);
            var objOrdersViewList = objuOrderList.Select(orders => (whOrderViewModel)orders);
            return View("Index", objOrdersViewList);
        }

        public IActionResult ProcessSpecificPurchase(int id)
        {
            try
            {
                _whOrderService.DeliveryProductToWh(id);
                return RedirectToAction(nameof(PurchaseQueue));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        public IActionResult ProcessNextPurchase()
        {
            try
            {
                _whOrderService.DeliveryProductToWh();
                return RedirectToAction(nameof(PurchaseQueue));
            }
            catch(ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        public IActionResult DeliverySpecificOrder(int id)
        {
            try
            {
                _whOrderService.DeliveryProductToUser(id);
                return RedirectToAction(nameof(DeliveryQueue));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        public IActionResult DeliveryNextOrder()
        {
            try
            {
                _whOrderService.DeliveryProductToUser();
                return RedirectToAction(nameof(DeliveryQueue));
            }
            catch (ValidationException ex)
            {
                //return Content(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult whOrderDetail(int? id)
        {
            try
            {
                DetailwhOrderDTO order = _whOrderService.GetDetailwhOrder(id);
                return View((DetailwhOrderViewModel)order);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
