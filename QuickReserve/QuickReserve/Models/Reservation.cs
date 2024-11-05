using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    internal class Reservation
    {
        public long ReservationId { get; set; }
        public long UserId { get; set; }
        public long TableId { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int GuestCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
