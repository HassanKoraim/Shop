using Shop.DataAccess.Data;
using Shop.Entities.Models;
using Shop.Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shop.DataAccess.Implementation
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Category category)
        {
            var categoryToUpdate = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.Name = category.Name;
                categoryToUpdate.Description = category.Description;
                categoryToUpdate.CreatedTime = category.CreatedTime;
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Category not found");
            }
        }
    }
}
