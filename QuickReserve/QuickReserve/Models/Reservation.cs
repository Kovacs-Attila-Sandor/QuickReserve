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
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int GuestCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public Reservation() { }
    }
}
