using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using myshop.Utilities;
using Shop.DataAccess.Implementation;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace Shop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int? page)
        {
            var PageNumber = page ?? 1;
            var PageSize = 8;
            var products = _unitOfWork.Product.GetAll().ToPagedList(PageNumber, PageSize);
            return View(products);
        }
        [HttpGet]
        public IActionResult Details(int ProductId)
        {
            if(ProductId != null)
            {
                ShoppingCart shopingCart = new ShoppingCart()
                {
                   // ProductId = ProductId,
                    Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == ProductId, includeWord: "Category"),
                    Count = 0
                };
               return View(shopingCart);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cart.ApplicationUserId = claim.Value;
            ShoppingCart cartObj = _unitOfWork.ShoppingCart.
                GetFirstOrDefault(x => x.ApplicationUserId == claim.Value && x.ProductId == cart.ProductId);
            if (cartObj == null)
            {
                _unitOfWork.ShoppingCart.Add(cart);
                _unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.SessionKey,
                    _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count());
            }
            else
            {
                _unitOfWork.ShoppingCart.IncreaseCount(cartObj, cart.Count);
                _unitOfWork.Complete();
            }
            return RedirectToAction("Index" , "Cart");
        }
    }
}
