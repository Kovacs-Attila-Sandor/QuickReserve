using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    public class RestaurantHours
    {
        public string Day { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public bool IsClosed { get; set; }
    }
}
