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
        public string ReservationDate { get; set; } // ISO 8601 formátum
        public string ReservationTime { get; set; } // Pl. "HH:mm"
        public int GuestCount { get; set; }
        public string CreatedAt { get; set; } // ISO 8601 formátum
    }
}
