using Microsoft.AspNetCore.Mvc;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using Shop.Entities.ViewModels;

namespace Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
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
                orderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderid, includeWord:"OrderHeader,Product")
            };
            return View(orderVM);
        }
    }
}
