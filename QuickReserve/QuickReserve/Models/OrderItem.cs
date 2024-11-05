using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    internal class OrderItem
    {
        public long OrderItemId { get; set; }
        public long OrderId { get; set; }
        public long FoodId { get; set; }
        public int Quantity { get; set; }
        public double Subtotal { get; set; }
    }
}
