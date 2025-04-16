using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Xamarin.Forms;
using System.Linq;

namespace QuickReserve.Models
{
    public class Food
    {
        public string FoodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public string Picture { get; set; }
        public bool IsAvailable { get; set; }
        public int PreparationTime { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Allergens { get; set; }
        public double Calories { get; set; }
        public List<string> Tags { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public int OrderCount { get; set; }
        public List<Rating> Ratings { get; set; } = new List<Rating>();
        public double Weight { get; set; }

        [JsonIgnore]
        public ImageSource ImageSource { get; set; }

        [JsonIgnore]
        public double Rating => Ratings.Any() ? Ratings.Average(r => r.Value) : 0;
    }
}
