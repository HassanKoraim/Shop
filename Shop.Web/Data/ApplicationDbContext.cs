using Microsoft.EntityFrameworkCore;
using Shop.Web.Models;

namespace Shop.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }

    }
}
