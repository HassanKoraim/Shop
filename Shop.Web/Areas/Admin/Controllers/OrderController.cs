using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using Shop.Entities.ViewModels;
using myshop.Utilities;
using Stripe;

namespace Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVm { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork; 
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetData()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = _unitOfWork.OrderHeader.GetAll(includeWord: "ApplicationUser");
            return Json(new { data = orderHeaders });
        }
        [HttpGet]
        public IActionResult Details(int orderid)
        {
            OrderVM orderVM = new OrderVM()
            {
                orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderid, includeWord: "ApplicationUser"),
                orderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderid, includeWord:"product")
            };
            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var orderfromdb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVm.orderHeader.Id);
            orderfromdb.Name = orderVm.orderHeader.Name;
            orderfromdb.Phone = orderVm.orderHeader.Phone;
            orderfromdb.Address = orderVm.orderHeader.Address;
            orderfromdb.PaymentStatus = orderVm.orderHeader.PaymentStatus;
            orderfromdb.City = orderVm.orderHeader.City;
            if(orderfromdb.Carrier != null)
            {
                orderfromdb.Carrier = orderVm.orderHeader.Carrier;
            }
            if(orderfromdb.TrackingNumber != null)
            {
                orderfromdb.TrackingNumber = orderVm.orderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderfromdb);
            _unitOfWork.Complete();
            TempData["Update"] = "Item has Updated Successfully";    
            return RedirectToAction("Details" , "Order" , new {orderid = orderfromdb.Id});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProccess()
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderVm.orderHeader.Id, SD.Proccessing, null);
            _unitOfWork.Complete();
            TempData["Update"] = "Order Status has Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderid = orderVm.orderHeader.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartShip()
        {
            var orderfromdb = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderVm.orderHeader.Id);
            orderfromdb.TrackingNumber = orderVm.orderHeader.TrackingNumber; 
            orderfromdb.Carrier = orderVm.orderHeader.Carrier; 
            orderfromdb.OrderStatus = SD.Shipped; 
            orderfromdb.OrderDate = DateTime.Now; 
            _unitOfWork.OrderHeader.Update(orderfromdb);
            _unitOfWork.Complete();
            TempData["Update"] = "Order has Shipped Successfully";
            return RedirectToAction("Details", "Order", new { orderid = orderVm.orderHeader.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            var orderfromdb = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderVm.orderHeader.Id);
            if(orderfromdb.PaymentStatus == SD.Approve)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderfromdb.PaymentIntendId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                _unitOfWork.OrderHeader.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Refund);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Cancelled);
            }
            _unitOfWork.Complete();
            TempData["Update"] = "Order has Cancelled Successfully";
            return RedirectToAction("Details", "Order", new { orderid = orderVm.orderHeader.Id });
        }
    }
}
