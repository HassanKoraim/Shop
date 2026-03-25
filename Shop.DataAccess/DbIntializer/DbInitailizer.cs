using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.DataAccess.Data;
using Shop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myshop.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shop.DataAccess.DbIntializer
{
    public class DbInitailizer : IDbInitializer
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitailizer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public void Initialize()
        {
            // Migration
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {

                throw;
            }

            // Roles
            if (!_roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.EditorRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();
               
                // User
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "Admin@myshop.com",
                    Email = "Admin@myshop.com",
                    Name = "Administrator",
                    PhoneNumber = "1234567890",
                    Address = "Alex",
                    City = "alex"
                }, "Admin_123").GetAwaiter().GetResult();
                ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "Admin@myshop.com");
                _userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
