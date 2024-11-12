using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public string ReservationId { get; set; }
        public double TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }
    }
}
