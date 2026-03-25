using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess.Data;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using System.Linq.Expressions;
using X.PagedList.Extensions;

namespace Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int? page)
        {
            int PageNumber = page ?? 1;
            int PageSize = 10;
            var categories = _unitOfWork.Category.GetAll().ToPagedList(PageNumber, PageSize);
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
                var category = _unitOfWork.Category.GetFirstOrDefault(predicate);
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
       /* [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id != null || id != 0)
            {
                Expression<Func<Category, bool>> predicate = c => c.Id == id;
                var category = _unitOfWork.Category.GetFirstOrDefault(predicate,null);
                if (category != null)
                {
                    return View(category);
                }
            }
            return NotFound();
        }*/

        [HttpDelete]
      //  [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Category category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            if (category != null)
            {
                Expression<Func<Category, bool>> predicate = c => c.Id == category.Id;
                _unitOfWork.Category.Remove(predicate);
                _unitOfWork.Complete();
               // TempData["Delete"] = $"{category.Name} has deleted successfully";
                return Json(new { success = true, message = "Category has been deleted successfully" });
                //return RedirectToAction(nameof(Index));
            }
            return Json(new { success = false, message = "Error while deleting" });
        }


    }
}
