using Microsoft.AspNetCore.Mvc;
using Shop.Web.Data;
using Shop.Web.Models;

namespace Shop.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;   
        }
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
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
                    _context.Categories.Add(category);
                    _context.SaveChanges();
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
                var category = _context.Categories.Find(id);
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
                _context.Categories.Update(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if(id != null || id != 0)
            {
                var category = _context.Categories.Find(id);
                if(category != null)
                {
                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return NotFound();
        }


    }
}
