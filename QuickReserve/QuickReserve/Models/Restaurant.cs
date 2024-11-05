using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    internal class Restaurant
    {
        public long RestaurantId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public double Rating { get; set; }
        public string Products { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public long LocationId { get; set; }
    }
}
