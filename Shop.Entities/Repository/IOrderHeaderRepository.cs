using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Entities.Models;

namespace Shop.Entities.Repository
{
    public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
        void UpdateStatus(int Id, string? OrderStatus, string? PaymentStatus);
    }
}
