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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Product product)
        {
            var productToUpdate = _context.Products.FirstOrDefault(c => c.Id == product.Id);
            if (productToUpdate != null)
            {
                productToUpdate.Name = product.Name;
                productToUpdate.Description = product.Description;
                productToUpdate.Price = product.Price;
                productToUpdate.CategoryId = product.CategoryId;
                productToUpdate.Category = product.Category;
                productToUpdate.Img = product.Img;
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Category not found");
            }
        }
    }
}
