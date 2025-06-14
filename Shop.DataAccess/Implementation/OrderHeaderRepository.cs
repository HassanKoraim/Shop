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
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        
        public void Update(OrderHeader orderHeader)
        {
            
            var orderHeaderToUpdate = _context.OrderHeaders.FirstOrDefault(o => o.Id == orderHeader.Id);
            if(orderHeaderToUpdate != null)
            {
                _context.OrderHeaders.Update(orderHeader);
            }
            else
            {
                throw new Exception("Order Header not found");
            }
        }

        public void UpdateStatus(int Id, string OrderStatus, string PaymentStatus)
        {
            var orderHeader = _context.OrderHeaders.FirstOrDefault(o => o.Id == Id);
            if(orderHeader != null)
            {
                orderHeader.OrderStatus = OrderStatus;
                orderHeader.PaymentStatus = PaymentStatus;
            }
            else
            {
                throw new Exception("Order Header not found");
            }
        }
    }
}
