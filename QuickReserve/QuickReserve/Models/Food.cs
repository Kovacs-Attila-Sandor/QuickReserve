using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace QuickReserve.Models
{
    public class Food
    {
        public string FoodId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Picture { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}
