using Microsoft.AspNetCore.Mvc;
using Shop.DataAccess.Data;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using Shop.Entities.ViewModels;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

namespace Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        //[Route("/")]
        //[Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetData()
        {
            var products = _unitOfWork.Product.GetAll(includeWord: "Category");
            return Json(new {data = products});
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
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
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
        [ActionName("Edit")]
        public IActionResult Edit(int? id)
        {
            if(id != null || id != 0)
            {
                Expression<Func<Product,bool>> predicate = c => c.Id == id;
                var product = _unitOfWork.Product.GetFirstOrDefault(predicate);
                if (product != null)
                {
                    ProductVM productVM = new ProductVM()
                    {
                        Product = product,
                        CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                        {
                            Text = x.Name,
                            Value = x.Id.ToString()
                        })
                    };
                    return View(productVM);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    if(productVM.Product.Img != null)
                    {
                        var oldImagePath = Path.Combine(rootPath, productVM.Product.Img.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
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
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Complete();
                TempData["Update"] = $"{productVM.Product.Name} has Edited successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productVM);
        }
        /*[HttpGet]
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
        }*/

        [HttpDelete]
        //[ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {
            Product product = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == id);
            if (product != null)
            {
                Expression<Func<Product, bool>> predicate = c => c.Id == product.Id;
                _unitOfWork.Product.Remove(predicate);
                string rootPath = _webHostEnvironment.WebRootPath;
                var oldImagePath = Path.Combine(rootPath, product.Img.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _unitOfWork.Complete();
                return Json(new { success = true, message = "File has been deleted successfully" });
            }

            return Json(new {success = false, message = "Error while deleting" });
        }


    }
}
