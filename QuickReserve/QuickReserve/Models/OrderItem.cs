using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    public class OrderItem
    {
        public string OrderItemId { get; set; }
        public string FoodId { get; set; }
        public int Quantity { get; set; }
        public double Subtotal { get; set; }
    }
}
