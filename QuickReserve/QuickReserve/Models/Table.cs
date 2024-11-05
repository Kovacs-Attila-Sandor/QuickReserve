using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    internal class Table
    {
        public long TableId { get; set; }
        public long RestaurantId { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string AvailabilityStatus { get; set; }
    }
}
