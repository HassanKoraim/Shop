using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Entities.Models;

namespace Shop.Entities.Repository
{
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        int IncreaseCount(ShoppingCart shoppingCart, int count);
        int DecreaseCount(ShoppingCart shoppingCart, int count);
    }
}
