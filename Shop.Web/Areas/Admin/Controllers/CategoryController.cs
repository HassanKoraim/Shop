using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess.Data;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using System.Linq.Expressions;

namespace Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
                if(ModelState.IsValid)
                {
                    _unitOfWork.Category.Add(category);
                    _unitOfWork.Complete();
                    TempData["Create"] = $"{category.Name} has Created successfully";
                return RedirectToAction(nameof(Index));
                }
            return View(category);
        }

        // Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id != null || id != 0)
            {
                Expression<Func<Category,bool>> predicate = c => c.Id == id;
                var category = _unitOfWork.Category.GetFirstOrDefualt(predicate);
                if (category != null)
                {  
                      return View(category);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Complete();
                TempData["Update"] = $"{category.Name} has Edited successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id != null || id != 0)
            {
                Expression<Func<Category, bool>> predicate = c => c.Id == id;
                var category = _unitOfWork.Category.GetFirstOrDefualt(predicate,null);
                if (category != null)
                {
                    return View(category);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category category)
        {
            if (category != null)
            {
                Expression<Func<Category, bool>> predicate = c => c.Id == category.Id;
                _unitOfWork.Category.Remove(predicate);
                _unitOfWork.Complete();
                TempData["Delete"] = $"{category.Name} has deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }


    }
}
