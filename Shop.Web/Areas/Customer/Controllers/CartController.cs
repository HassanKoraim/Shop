using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Utilities;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using Shop.Entities.ViewModels;
using Stripe;
using Stripe.Checkout;
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
        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var ShoppingCartVM = new ShoppingCartVM()
            {
                CartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeWord: "Product"),
                OrderHeader = new OrderHeader()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.Phone = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var item in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM.CartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeWord: "Product");


            shoppingCartVM.OrderHeader.OrderStatus = SD.Pending;
            shoppingCartVM.OrderHeader.PaymentStatus = SD.Pending;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;


            foreach (var item in shoppingCartVM.CartsList)
            {
                shoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            }

            _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Complete();

            foreach (var item in shoppingCartVM.CartsList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId =shoppingCartVM.OrderHeader.Id,
                    price = item.Product.Price,
                    count = item.Count
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Complete();
            }
            //https://c8b7-41-236-57-177.ngrok-free.app/
            var domain = "https://localhost:7004/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
            };

            foreach (var item in shoppingCartVM.CartsList)
            {
                var sessionlineoption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionlineoption);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            shoppingCartVM.OrderHeader.SessionId = session.Id;
            shoppingCartVM.OrderHeader.PaymentIntendId = session.SetupIntentId;

            _unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.Approve, SD.Approve);
                orderHeader.PaymentIntendId = session.PaymentIntentId;
                _unitOfWork.Complete();
            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId ==orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Complete();
            return View(id);
                
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
