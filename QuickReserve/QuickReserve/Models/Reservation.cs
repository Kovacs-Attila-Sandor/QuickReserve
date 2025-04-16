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
        public List<OrderedFoodsAndQuantity> FoodIdsAndQuantity { get; set; }
        public string Status { get; set; }
        public string TableNumber { get; set; }
        public string UserName { get; set; }

        [JsonIgnore]
        public bool HasFoods => FoodIdsAndQuantity != null && FoodIdsAndQuantity.Count > 0;
        [JsonIgnore]
        public bool IsDoneVisible => Status == "In progress";
        
    }
    public class OrderedFoodsAndQuantity {
        public string FoodId { get; set; }
        public int Quantity { get; set; }
    }
}
