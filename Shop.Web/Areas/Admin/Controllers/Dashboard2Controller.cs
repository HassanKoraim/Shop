using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Entities.Models;
using Shop.Entities.Repository;

namespace Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class Dashboard2Controller : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public Dashboard2Controller(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<OrderHeader> orders = _unitOfWork.OrderHeader.GetAll();
            long totalRevenue = (long) orders.Sum(o => o.TotalPrice);
            long ordersNumber = orders.Count();
            long avgOrderValue = totalRevenue > 0 ? totalRevenue  / ordersNumber : 0;
            ViewBag.totalRevenue = totalRevenue;
            ViewBag.ordersNumber = ordersNumber;
            ViewBag.avgOrderValue = avgOrderValue;
            return View();
        }
    }
}
