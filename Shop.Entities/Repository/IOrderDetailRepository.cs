using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Entities.Models;

namespace Shop.Entities.Repository
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
