using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Utilities;
using Shop.DataAccess.Data;
using Shop.DataAccess.Implementation;
using Shop.Entities.Repository;
using Shop.Entities.ViewModels;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int? page)
        {
            //_context.Users();
            int PageNumber = page ?? 1;
            int PageSize = 10;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;
            return View(_context.ApplicationUsers.Where(x => x.Id != userId).ToList().ToPagedList(PageNumber,PageSize));
        }
        public IActionResult LockUnlock(string? id)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            if(user.LockoutEnd == null || user.LockoutEnd < DateTime.Now) // The lock is open
            {
                // The user will locked for a year ago
                user.LockoutEnd = DateTime.Now.AddYears(1);
                TempData["Delete"] = $"{user.Name} has Locked";
            }
            else  // The lock is lock
            {
                user.LockoutEnd = null;
                TempData["Create"] = $"{user.Name} has UnLocked";
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if(user != null)
            {
                _unitOfWork.ApplicationUser.Remove(user);
                _unitOfWork.Complete();
                return Json(new { success = true, message = "User has been deleted successfully" });
            }
            return Json(new { success = false, message = "Error while deleting" });
        }
    }
}
