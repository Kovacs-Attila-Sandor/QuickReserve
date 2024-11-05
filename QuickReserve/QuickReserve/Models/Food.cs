using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    internal class Food
    {
        public long FoodId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public long RestaurantId { get; set; }
    }
}
