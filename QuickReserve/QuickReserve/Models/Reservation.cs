using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    public class Reservation
    {
        public string ReservationId { get; set; }
        public string UserId { get; set; }
        public string RestaurantId { get; set; }
        public string TableId { get; set; }
        public string ReservationDateTime { get; set; }  
        public int GuestCount { get; set; }
        public string CreatedAt { get; set; } 
        public List<Food> Foods { get; set; }
        public string Status { get; set; }
        public string TableNumber { get; set; }

        [JsonIgnore]
        public bool HasFoods => Foods != null && Foods.Count > 0;
        [JsonIgnore]
        public bool IsDoneVisible => Status == "In progress";
    }
}
