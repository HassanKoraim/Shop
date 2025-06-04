using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using Shop.Entities.ViewModels;
using System.Security.Claims;

namespace Shop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //cart.ApplicationUserId = claim.Value;
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
               CartsList =  _unitOfWork.ShoppingCart.
                GetAll(x => x.ApplicationUserId == claim.Value , includeWord:"Product"),
            };
            foreach(var item in shoppingCartVM.CartsList)
            {
                shoppingCartVM.TotalPrice += (item.Count * item.Product.Price);
            }
            //IEnumerable<ShoppingCart> cartObjs = _unitOfWork.ShoppingCart.
            //    GetAll(x => x.ApplicationUserId == claim.Value , includeWord:"Product");

            return View(shoppingCartVM);
        }

        public IActionResult Plus(int CartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == CartId);
            _unitOfWork.ShoppingCart.IncreaseCount(cart,1);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }
        public IActionResult Minus(int CartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == CartId);
            if(cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Complete();
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //cart.ApplicationUserId = claim.Value;
                ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
                {
                    CartsList = _unitOfWork.ShoppingCart.
                    GetAll(x => x.ApplicationUserId == claim.Value, includeWord: "Product"),
                };
                if(shoppingCartVM.CartsList.Count() == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                _unitOfWork.ShoppingCart.DecreaseCount(cart, 1);
            }
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }
        public ActionResult Remove(int CartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == CartId);
            if(cart != null)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Complete();
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //cart.ApplicationUserId = claim.Value;
                ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
                {
                    CartsList = _unitOfWork.ShoppingCart.
                    GetAll(x => x.ApplicationUserId == claim.Value, includeWord: "Product"),
                };
                if (shoppingCartVM.CartsList.Count() == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index");
        }
    }
}
