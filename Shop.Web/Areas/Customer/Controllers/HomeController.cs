using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess.Implementation;
using Shop.Entities.Repository;
using Shop.Entities.ViewModels;

namespace Shop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll();
            return View(products);
        }
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if(id != null)
            {
                ShopingCart shopingCart = new ShopingCart()
                {
                    product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeWord: "Category"),
                    Count = 0
                };
               return View(shopingCart);
            }
            return NotFound();
        }
    }
}
