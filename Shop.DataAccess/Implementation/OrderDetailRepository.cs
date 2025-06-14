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
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        
        public void Update(OrderDetail orderDetail)
        {
            
            var orderDetailToUpdate = _context.OrderDetails.FirstOrDefault(o => o.Id == orderDetail.Id);
            if(orderDetailToUpdate != null)
            {
                _context.OrderDetails.Update(orderDetail);
            }
            else
            {
                throw new Exception("Order Header not found");
            }
        }

    }
}
