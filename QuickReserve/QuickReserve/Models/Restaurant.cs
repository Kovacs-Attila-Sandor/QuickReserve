using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
        public List<string> Products { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public Location Location { get; set; }
        public List<Table> Tables { get; set; }
        public List<Food> Foods { get; set; }

        public Restaurant()
        {
            Products = new List<string>();
            Tables = new List<Table>();
            Foods = new List<Food>();
        }
    }
}
