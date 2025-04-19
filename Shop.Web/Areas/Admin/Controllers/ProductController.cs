using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess.Data;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using Shop.Entities.ViewModels;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var Products = _unitOfWork.Product.GetAll();
            return View(Products);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(product);
                _unitOfWork.Complete();
                TempData["Create"] = $"{product.Name} has Created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id != null || id != 0)
            {
                Expression<Func<Product,bool>> predicate = c => c.Id == id;
                var product = _unitOfWork.Product.GetFirstOrDefualt(predicate);
                if (product != null)
                {  
                      return View(product);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(product);
                _unitOfWork.Complete();
                TempData["Update"] = $"{product.Name} has Edited successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id != null || id != 0)
            {
                Expression<Func<Product, bool>> predicate = c => c.Id == id;
                var product = _unitOfWork.Product.GetFirstOrDefualt(predicate,null);
                if (product != null)
                {
                    return View(product);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Product product)
        {
            if (product != null)
            {
                Expression<Func<Product, bool>> predicate = c => c.Id == product.Id;
                _unitOfWork.Product.Remove(predicate);
                _unitOfWork.Complete();
                TempData["Delete"] = $"{product.Name} has deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }


    }
}
