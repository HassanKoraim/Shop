using System;
using System.ComponentModel.DataAnnotations;
using Shop.Entities.Models;

namespace Shop.Entities.ViewModels
{
    public class ShopingCart
    {
        public Product product { get; set; }
        [Range(1, 100, ErrorMessage = "Count must be between 1 and 100")]
        public int Count { get; set; }
    }
}
