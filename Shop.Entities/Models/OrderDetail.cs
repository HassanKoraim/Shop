using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Entities.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ValidateNever]
        public OrderHeader orderHeader { get; set; }
        public int ProductId { get; set; }
        [ValidateNever]
        public Product product { get; set; }

        public int count { get; set; }
        public decimal price { get; set; }
    }
}
