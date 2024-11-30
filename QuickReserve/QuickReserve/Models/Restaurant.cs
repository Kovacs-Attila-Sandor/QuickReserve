using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    public class Restaurant
    {

        public string RestaurantId { get; set; }
        public string Name { get; set; }
        public Location Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public double Rating { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public List<Food> Foods { get; set; } = new List<Food>();
        public List<Table> Tables { get; set; } = new List<Table>();

        public string imgstring { get; set; }
    }
}
