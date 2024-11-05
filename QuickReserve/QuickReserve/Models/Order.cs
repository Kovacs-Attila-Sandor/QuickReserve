using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    internal class Order
    {
        public long OrderId { get; set; }
        public long UserId { get; set; }
        public long ReservationId { get; set; }
        public double TotalAmount { get; set; }
        public string OrderStatus { get; set; }
    }
}
