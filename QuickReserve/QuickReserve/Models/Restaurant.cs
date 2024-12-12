using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace QuickReserve.Models
{
    public class Restaurant
    {
        public string RestaurantId { get; set; }
        public string Name { get; set; }
        public RestaurantLocation Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public double Rating { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public List<RestaurantHours> Hours { get; set; }

        // Több kép Base64 kódolva
        public List<string> ImageBase64List { get; set; } = new List<string>();
        public string FirstImageBase64
        {
            get
            {
                return ImageBase64List != null && ImageBase64List.Count > 0
                    ? ImageBase64List[0]
                    : null;
            }
        }
        public ImageSource ImageSourceUri { get; set; }

        // Dekódolt képek ImageSource típusban
        public List<ImageSource> ImageSourceList { get; set; } = new List<ImageSource>();

        public List<Food> Foods { get; set; } = new List<Food>();
        public List<Table> Tables { get; set; } = new List<Table>();
    }
}
