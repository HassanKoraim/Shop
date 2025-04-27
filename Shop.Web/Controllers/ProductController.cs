using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.DataAccess.Data;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using Shop.Entities.ViewModels;
using System.Linq.Expressions;

namespace Shop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult GetData()
        {
            var products = _unitOfWork.Product.GetAll(includeWord:"Category");
            return Json(new { data = products });
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            // Anthor way to add CategoryList to Create view
            /*ViewBag.CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });*/
            return View(productVM);           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {
            if(ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var upload = Path.Combine(rootPath, @"images/products");
                    var ext = Path.GetExtension(file.FileName);
                    using (var filestream = new FileStream(Path.Combine(upload, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    // Save the image path to the database
                    productVM.Product.Img = @"\images\products\" + filename + ext;
                }
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Complete();
                TempData["Create"] = $"{productVM.Product.Name} has Created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productVM);
        }

        // Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id != null || id != 0)
            {
                Expression<Func<Product,bool>> predicate = c => c.Id == id;
                var product = _unitOfWork.Product.GetFirstOrDefault(predicate);
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
                var product = _unitOfWork.Product.GetFirstOrDefault(predicate,null);
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
