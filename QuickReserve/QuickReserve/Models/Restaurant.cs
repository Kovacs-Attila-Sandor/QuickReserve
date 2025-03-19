using Newtonsoft.Json;
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
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public List<RestaurantHours> Hours { get; set; }
        public string UserId { get; set; }
        public List<string> Categorys { get; set; }

        // Több kép Base64 kódolva
        public List<string> ImageBase64List { get; set; } = new List<string>();
        [JsonIgnore]
        public string FirstImageBase64
        {
            get
            {
                return ImageBase64List != null && ImageBase64List.Count > 0
                    ? ImageBase64List[0]
                    : null;
            }
        }

        [JsonIgnore]
        public ImageSource ImageSourceUri { get; set; }

        // Dekódolt képek ImageSource típusban
        [JsonIgnore]
        public List<ImageSource> ImageSourceList { get; set; } = new List<ImageSource>();

        public List<Food> Foods { get; set; } = new List<Food>();
        public List<Table> Tables { get; set; } = new List<Table>();
    }
}
