using System;
using System.Collections.Generic;
using System.Text;

namespace QuickReserve.Models
{
    internal class Location
    {
        public long LocationId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
    }
}
